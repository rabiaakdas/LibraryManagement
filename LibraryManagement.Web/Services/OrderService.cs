using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LibraryManagement.Web.Areas.Admin.Models;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Models;
using LibraryManagement.Web.Models.Api;
using LibraryManagement.Web.Mappings;
using LibraryManagement.Web.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Web.Services
{
    /// <summary>
    /// Provides order business logic for checkout, user history, admin management, and dashboard summaries.
    /// </summary>
    public class OrderService : IOrderService
    {
        private static readonly List<string> StatusOptions = new()
        {
            "Pending",
            "Preparing",
            "Shipped",
            "Delivered",
            "Cancelled"
        };

        private readonly IOrderRepository _orders;
        private readonly IBookRepository _books;
        private readonly IUserRepository _users;
        private readonly IAddressRepository _addresses;
        private readonly IMapper _mapper;
        private readonly IGenreRepository _genres;
        private readonly IReviewRepository _reviews;
        private readonly IBookService _bookService;
        private readonly ICouponService _couponService;
        private readonly IEmailService _emailService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orders,
            IBookRepository books,
            IUserRepository users,
            IAddressRepository addresses,
            IGenreRepository genres,
            IReviewRepository reviews,
            IBookService bookService,
            ICouponService couponService = null,
            IEmailService emailService = null,
            IMapper mapper = null,
            ILogger<OrderService> logger = null)
        {
            _orders = orders;
            _books = books;
            _users = users;
            _addresses = addresses;
            _genres = genres;
            _reviews = reviews;
            _bookService = bookService;
            _couponService = couponService;
            _emailService = emailService;
            _mapper = mapper ?? new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();
            _logger = logger;
        }

        public async Task<(bool Success, string Error)> CheckoutAsync(List<CartItemViewModel> cart, string userId)
        {
            var model = new CheckoutViewModel
            {
                CartItems = cart,
                PaymentMethod = "Kapıda ödeme"
            };

            return await CheckoutAsync(model, userId);
        }

        public async Task<(bool Success, string Error)> CheckoutAsync(CheckoutViewModel model, string userId)
        {
            var cart = model.CartItems;
            if (cart == null || !cart.Any())
            {
                return (false, "Sepetiniz boş!");
            }

            if (!model.AddressId.HasValue)
            {
                return (false, "Teslimat adresi secmelisiniz.");
            }

            var address = await _addresses.GetByIdAsync(model.AddressId.Value);
            if (address == null || address.UserId != userId)
            {
                return (false, "Gecerli bir teslimat adresi secmelisiniz.");
            }

            var orderItems = new List<OrderItem>();
            decimal subTotal = 0;

            foreach (var item in cart)
            {
                var book = await _books.GetByIdAsync(item.BookId);
                if (book == null)
                {
                    continue;
                }

                if (book.Stock < item.Quantity)
                {
                    return (false, $"{book.Title} için stok yetersiz. İstenen adet: {item.Quantity}, mevcut stok: {book.Stock}. Lütfen sepetinizi güncelleyin.");
                }

                var lineTotal = book.Price * item.Quantity;
                subTotal += lineTotal;

                book.Stock -= item.Quantity;
                LogStockWarningIfNeeded(book);
                orderItems.Add(new OrderItem
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    Price = book.Price,
                    Quantity = item.Quantity,
                    TotalPrice = lineTotal
                });
            }

            if (!orderItems.Any())
            {
                return (false, "Sepetinizde siparişe eklenebilecek ürün bulunamadı.");
            }

            var shippingFee = CalculateShippingFee(subTotal);
            decimal discountAmount = 0;
            var couponCode = string.Empty;

            if (!string.IsNullOrWhiteSpace(model.CouponCode) && _couponService != null)
            {
                var couponResult = await _couponService.ValidateCouponAsync(model.CouponCode, subTotal);
                if (!couponResult.Success)
                {
                    return (false, couponResult.Message);
                }

                couponCode = couponResult.CouponCode;
                discountAmount = couponResult.DiscountAmount;
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = subTotal,
                AddressId = address.Id,
                PaymentMethod = model.PaymentMethod,
                ShippingFee = shippingFee,
                CouponCode = couponCode,
                DiscountAmount = discountAmount,
                GrandTotal = subTotal + shippingFee - discountAmount,
                Status = "Pending",
                Items = orderItems
            };

            _orders.Add(order);
            if (!string.IsNullOrWhiteSpace(couponCode) && _couponService != null)
            {
                await _couponService.MarkAsUsedAsync(couponCode);
            }
            await _orders.SaveChangesAsync();

            _logger?.LogInformation(
                "Yeni siparis olusturuldu. OrderId: {OrderId}, UserId: {UserId}, PaymentMethod: {PaymentMethod}, GrandTotal: {GrandTotal}, ItemCount: {ItemCount}",
                order.Id,
                order.UserId,
                order.PaymentMethod,
                order.GrandTotal,
                order.Items.Sum(i => i.Quantity));

            if (!string.IsNullOrWhiteSpace(order.CouponCode))
            {
                _logger?.LogInformation(
                    "Kupon kullanılarak sipariş oluşturuldu. OrderId: {OrderId}, CouponCode: {CouponCode}, DiscountAmount: {DiscountAmount}",
                    order.Id,
                    order.CouponCode,
                    order.DiscountAmount);
            }

            await SendOrderCreatedEmailAsync(order);

            return (true, null);
        }

        public static decimal CalculateShippingFee(decimal subTotal)
        {
            return subTotal >= 500 ? 0 : 49.90m;
        }

        public async Task<List<OrderViewModel>> GetUserOrdersAsync(string userId)
        {
            return await _orders.Query()
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderViewModel
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    ShippingFee = o.ShippingFee,
                    CouponCode = o.CouponCode,
                    DiscountAmount = o.DiscountAmount,
                    GrandTotal = o.GrandTotal == 0 ? o.TotalAmount : o.GrandTotal,
                    PaymentMethod = o.PaymentMethod,
                    AddressTitle = o.Address == null ? string.Empty : o.Address.Title,
                    AddressDetail = o.Address == null
                        ? string.Empty
                        : $"{o.Address.FullAddress}, {o.Address.District}/{o.Address.City} {o.Address.ZipCode}",
                    CargoCompany = o.CargoCompany,
                    TrackingNumber = o.TrackingNumber,
                    ShippedAt = o.ShippedAt,
                    DeliveredAt = o.DeliveredAt,
                    Status = o.Status,
                    Items = o.Items.Select(i => new OrderItemViewModel
                    {
                        Title = i.Title,
                        Quantity = i.Quantity,
                        Price = i.Price,
                        TotalPrice = i.TotalPrice
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<AdminOrderListViewModel> GetAdminOrderListAsync()
        {
            return new AdminOrderListViewModel
            {
                Orders = await _orders.QueryWithItems()
                    .OrderByDescending(o => o.OrderDate)
                    .Select(o => new AdminOrderListItemViewModel
                    {
                        Id = o.Id,
                        UserId = o.UserId,
                        OrderDate = o.OrderDate,
                        TotalAmount = o.TotalAmount,
                        Status = o.Status,
                        ItemCount = o.Items.Sum(i => i.Quantity)
                    })
                    .ToListAsync()
            };
        }

        public async Task<AdminOrderDetailsViewModel> GetAdminOrderDetailsAsync(int id)
        {
            var order = await _orders.GetByIdWithItemsAsync(id);
            return order == null ? null : new AdminOrderDetailsViewModel { Order = order };
        }

        public async Task<AdminOrderStatusViewModel> GetEditStatusModelAsync(int id)
        {
            var order = await _orders.GetByIdAsync(id);
            if (order == null)
            {
                return null;
            }

            return new AdminOrderStatusViewModel
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                CargoCompany = order.CargoCompany,
                TrackingNumber = order.TrackingNumber,
                StatusOptions = StatusOptions
            };
        }

        public async Task<(bool Success, string Error)> UpdateStatusAsync(int id, AdminOrderStatusViewModel model)
        {
            if (!StatusOptions.Contains(model.Status))
            {
                return (false, "Gecersiz siparis durumu.");
            }

            var order = await _orders.GetByIdAsync(id);
            if (order == null)
            {
                return (false, "Siparis bulunamadi.");
            }

            var previousStatus = order.Status;
            var previousCargoCompany = order.CargoCompany;
            var previousTrackingNumber = order.TrackingNumber;

            order.Status = model.Status;
            order.CargoCompany = model.CargoCompany?.Trim() ?? string.Empty;
            order.TrackingNumber = model.TrackingNumber?.Trim() ?? string.Empty;

            if (order.Status == "Shipped" && !order.ShippedAt.HasValue)
            {
                order.ShippedAt = DateTime.Now;
            }

            if (order.Status == "Delivered" && !order.DeliveredAt.HasValue)
            {
                order.DeliveredAt = DateTime.Now;
                order.ShippedAt ??= DateTime.Now;
            }

            await _orders.SaveChangesAsync();

            _logger?.LogInformation(
                "Admin siparis durumu degistirdi. OrderId: {OrderId}, NewStatus: {Status}",
                order.Id,
                order.Status);

            if (previousCargoCompany != order.CargoCompany || previousTrackingNumber != order.TrackingNumber)
            {
                _logger?.LogInformation(
                    "Admin kargo bilgisi ekledi/guncelledi. OrderId: {OrderId}, CargoCompany: {CargoCompany}, TrackingNumber: {TrackingNumber}",
                    order.Id,
                    order.CargoCompany,
                    order.TrackingNumber);
            }

            if (previousStatus != "Shipped" && order.Status == "Shipped")
            {
                _logger?.LogInformation("Siparis Shipped yapildi. OrderId: {OrderId}, ShippedAt: {ShippedAt}", order.Id, order.ShippedAt);
            }

            if (previousStatus != "Delivered" && order.Status == "Delivered")
            {
                _logger?.LogInformation("Siparis Delivered yapildi. OrderId: {OrderId}, DeliveredAt: {DeliveredAt}", order.Id, order.DeliveredAt);
            }

            await SendOrderStatusUpdatedEmailAsync(order);

            return (true, null);
        }

        private async Task SendOrderCreatedEmailAsync(Order order)
        {
            if (_emailService == null || string.IsNullOrWhiteSpace(order.UserId))
            {
                return;
            }

            var discountLine = order.DiscountAmount > 0
                ? $"İndirim: {order.DiscountAmount:C}\n"
                : string.Empty;

            var body =
                $"Merhaba {order.UserId},\n\n" +
                $"Kitap siparişiniz alındı.\n\n" +
                $"Sipariş No: {order.Id}\n" +
                $"Ara Toplam: {order.TotalAmount:C}\n" +
                $"Kargo Ücreti: {order.ShippingFee:C}\n" +
                discountLine +
                $"Genel Toplam: {order.GrandTotal:C}\n" +
                $"Ödeme Yöntemi: {order.PaymentMethod}\n" +
                $"Sipariş Durumu: {order.Status}\n";

            await _emailService.SendEmailAsync(order.UserId, "Kitap siparişiniz alındı", body);
        }

        private void LogStockWarningIfNeeded(Book book)
        {
            if (book.Stock <= 0)
            {
                _logger?.LogWarning(
                    "Kitap stogu 0'a dustu. BookId: {BookId}, Title: {Title}",
                    book.BookId,
                    book.Title);
                return;
            }

            if (book.Stock <= 5)
            {
                _logger?.LogInformation(
                    "Siparis sonrasi kitap stogu dusuk seviyeye dustu. BookId: {BookId}, Title: {Title}, Stock: {Stock}",
                    book.BookId,
                    book.Title,
                    book.Stock);
            }
        }

        private async Task SendOrderStatusUpdatedEmailAsync(Order order)
        {
            if (_emailService == null || string.IsNullOrWhiteSpace(order.UserId))
            {
                return;
            }

            var body =
                $"Merhaba,\n\n" +
                $"Sipariş durumunuz güncellendi.\n\n" +
                $"Sipariş No: {order.Id}\n" +
                $"Yeni Durum: {order.Status}\n";

            if (!string.IsNullOrWhiteSpace(order.CargoCompany))
            {
                body += $"Kargo Şirketi: {order.CargoCompany}\n";
            }

            if (!string.IsNullOrWhiteSpace(order.TrackingNumber))
            {
                body += $"Takip Numarası: {order.TrackingNumber}\n";
            }

            if (order.ShippedAt.HasValue)
            {
                body += $"Kargoya Verildi: {order.ShippedAt:dd.MM.yyyy HH:mm}\n";
            }

            await _emailService.SendEmailAsync(order.UserId, "Sipariş durumunuz güncellendi", body);
        }

        public async Task<AdminDashboardViewModel> GetDashboardAsync()
        {
            return new AdminDashboardViewModel
            {
                TotalBooks = await _books.Query().CountAsync(),
                TotalUsers = await _users.Query().CountAsync(),
                TotalOrders = await _orders.Query().CountAsync(),
                TotalGenres = await _genres.Query().CountAsync(),
                TotalReviews = await _reviews.Query().CountAsync(),
                TotalRevenue = await _orders.Query().SumAsync(o => (decimal?)o.TotalAmount) ?? 0,
                PendingOrders = await _orders.Query().CountAsync(o => o.Status == "Pending"),
                OutOfStockBooks = await _books.Query().CountAsync(b => b.Stock <= 0),
                LatestOrders = await _orders.Query()
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .Select(o => new DashboardOrderViewModel
                    {
                        Id = o.Id,
                        UserId = o.UserId,
                        OrderDate = o.OrderDate,
                        TotalAmount = o.TotalAmount,
                        Status = o.Status
                    })
                    .ToListAsync(),
                BestSellingBooks = await _orders.QueryItems()
                    .GroupBy(i => new { i.BookId, i.Title })
                    .OrderByDescending(g => g.Sum(i => i.Quantity))
                    .Take(5)
                    .Select(g => new DashboardBestSellerViewModel
                    {
                        Title = g.Key.Title,
                        TotalQuantity = g.Sum(i => i.Quantity),
                        TotalRevenue = g.Sum(i => i.TotalPrice)
                    })
                    .ToListAsync(),
                LatestReviews = await _reviews.QueryWithBookAndUser()
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(5)
                    .Select(r => new DashboardReviewViewModel
                    {
                        Id = r.Id,
                        BookTitle = r.Book.Title,
                        Username = r.User.Username,
                        Rating = r.Rating,
                        Comment = r.Comment,
                        CreatedAt = r.CreatedAt
                    })
                    .ToListAsync(),
                LowStockBooks = await _bookService.GetLowStockDashboardAsync()
            };
        }

        public async Task<List<OrderDto>> GetApiOrdersAsync()
        {
            var orders = await _orders.QueryWithItems()
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return _mapper.Map<List<OrderDto>>(orders);
        }

        public async Task<OrderDto> GetApiOrderAsync(int id)
        {
            var order = await _orders.QueryWithItems()
                .FirstOrDefaultAsync(o => o.Id == id);

            return order == null ? null : _mapper.Map<OrderDto>(order);
        }
    }
}
