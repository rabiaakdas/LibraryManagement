using LibraryManagement.Tests.Helpers;
using LibraryManagement.Web.Areas.Admin.Models;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Models;
using LibraryManagement.Web.Repositories;
using LibraryManagement.Web.Services;
using LibraryManagement.Web.Validators;
using Moq;

namespace LibraryManagement.Tests.Services;

public class OrderServiceTests
{
    private static OrderService CreateService(Mock<IOrderRepository> orderRepository, Mock<IEmailService>? emailService = null)
    {
        var bookRepository = new Mock<IBookRepository>();
        var userRepository = new Mock<IUserRepository>();
        var addressRepository = new Mock<IAddressRepository>();
        var genreRepository = new Mock<IGenreRepository>();
        var reviewRepository = new Mock<IReviewRepository>();
        var bookService = new Mock<IBookService>();

        orderRepository.Setup(r => r.Query()).Returns(AsyncQueryable.Create(new List<Order>()));
        orderRepository.Setup(r => r.QueryItems()).Returns(AsyncQueryable.Create(new List<OrderItem>()));
        bookRepository.Setup(r => r.Query()).Returns(AsyncQueryable.Create(new List<Book>()));
        userRepository.Setup(r => r.Query()).Returns(AsyncQueryable.Create(new List<User>()));
        genreRepository.Setup(r => r.Query()).Returns(AsyncQueryable.Create(new List<Genre>()));
        reviewRepository.Setup(r => r.Query()).Returns(AsyncQueryable.Create(new List<BookReview>()));

        return new OrderService(
            orderRepository.Object,
            bookRepository.Object,
            userRepository.Object,
            addressRepository.Object,
            genreRepository.Object,
            reviewRepository.Object,
            bookService.Object,
            null,
            emailService?.Object);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenOrderExists_UpdatesStatus()
    {
        var order = new Order { Id = 1, UserId = "user@test.com", Status = "Pending" };
        var orderRepository = new Mock<IOrderRepository>();
        orderRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);
        var service = CreateService(orderRepository);

        var result = await service.UpdateStatusAsync(1, new AdminOrderStatusViewModel { Id = 1, Status = "Preparing" });

        Assert.True(result.Success);
        Assert.Equal("Preparing", order.Status);
        orderRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenOrderDoesNotExist_ReturnsError()
    {
        var orderRepository = new Mock<IOrderRepository>();
        orderRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Order?)null);
        var service = CreateService(orderRepository);

        var result = await service.UpdateStatusAsync(99, new AdminOrderStatusViewModel { Id = 99, Status = "Shipped" });

        Assert.False(result.Success);
        Assert.Equal("Siparis bulunamadi.", result.Error);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenStatusShipped_SetsCargoInformationAndShippedAt()
    {
        var order = new Order { Id = 2, UserId = "user@test.com", Status = "Preparing" };
        var orderRepository = new Mock<IOrderRepository>();
        orderRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(order);
        var service = CreateService(orderRepository);

        var result = await service.UpdateStatusAsync(2, new AdminOrderStatusViewModel
        {
            Id = 2,
            Status = "Shipped",
            CargoCompany = "Yurtiçi Kargo",
            TrackingNumber = "TRK12345"
        });

        Assert.True(result.Success);
        Assert.Equal("Shipped", order.Status);
        Assert.Equal("Yurtiçi Kargo", order.CargoCompany);
        Assert.Equal("TRK12345", order.TrackingNumber);
        Assert.NotNull(order.ShippedAt);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenStatusDelivered_SetsDeliveredAt()
    {
        var order = new Order { Id = 3, UserId = "user@test.com", Status = "Shipped" };
        var orderRepository = new Mock<IOrderRepository>();
        orderRepository.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(order);
        var service = CreateService(orderRepository);

        var result = await service.UpdateStatusAsync(3, new AdminOrderStatusViewModel
        {
            Id = 3,
            Status = "Delivered",
            CargoCompany = "Yurtiçi Kargo",
            TrackingNumber = "TRK12345"
        });

        Assert.True(result.Success);
        Assert.Equal("Delivered", order.Status);
        Assert.NotNull(order.DeliveredAt);
    }

    [Fact]
    public void OrderValidator_WhenStatusShippedWithoutCargo_ReturnsValidationError()
    {
        var validator = new OrderValidator();

        var result = validator.Validate(new AdminOrderStatusViewModel { Status = "Shipped" });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(AdminOrderStatusViewModel.CargoCompany));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(AdminOrderStatusViewModel.TrackingNumber));
    }

    [Fact]
    public async Task CheckoutAsync_WhenOrderCreated_SendsEmail()
    {
        var orderRepository = new Mock<IOrderRepository>();
        var bookRepository = new Mock<IBookRepository>();
        var userRepository = new Mock<IUserRepository>();
        var addressRepository = new Mock<IAddressRepository>();
        var genreRepository = new Mock<IGenreRepository>();
        var reviewRepository = new Mock<IReviewRepository>();
        var bookService = new Mock<IBookService>();
        var emailService = new Mock<IEmailService>();

        bookRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Book { BookId = 1, Title = "Dune", Price = 100, Stock = 5 });
        addressRepository.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new Address { Id = 10, UserId = "user@test.com" });
        emailService.Setup(s => s.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        var service = new OrderService(
            orderRepository.Object,
            bookRepository.Object,
            userRepository.Object,
            addressRepository.Object,
            genreRepository.Object,
            reviewRepository.Object,
            bookService.Object,
            null,
            emailService.Object);

        var result = await service.CheckoutAsync(new CheckoutViewModel
        {
            AddressId = 10,
            PaymentMethod = "Kapıda ödeme",
            CartItems = new List<CartItemViewModel>
            {
                new() { BookId = 1, Quantity = 1 }
            }
        }, "user@test.com");

        Assert.True(result.Success);
        emailService.Verify(s => s.SendEmailAsync("user@test.com", "Kitap siparişiniz alındı", It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenStatusChanges_SendsEmail()
    {
        var order = new Order { Id = 4, UserId = "user@test.com", Status = "Pending" };
        var orderRepository = new Mock<IOrderRepository>();
        var emailService = new Mock<IEmailService>();
        orderRepository.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(order);
        emailService.Setup(s => s.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        var service = CreateService(orderRepository, emailService);

        var result = await service.UpdateStatusAsync(4, new AdminOrderStatusViewModel { Id = 4, Status = "Preparing" });

        Assert.True(result.Success);
        emailService.Verify(s => s.SendEmailAsync("user@test.com", "Sipariş durumunuz güncellendi", It.IsAny<string>()), Times.Once);
    }
    [Fact]
    public async Task CheckoutAsync_WhenBookIsOutOfStock_ReturnsStockError()
    {
        var orderRepository = new Mock<IOrderRepository>();
        var bookRepository = new Mock<IBookRepository>();
        var userRepository = new Mock<IUserRepository>();
        var addressRepository = new Mock<IAddressRepository>();
        var genreRepository = new Mock<IGenreRepository>();
        var reviewRepository = new Mock<IReviewRepository>();
        var bookService = new Mock<IBookService>();

        bookRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Book { BookId = 1, Title = "Dune", Price = 100, Stock = 0 });
        addressRepository.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new Address { Id = 10, UserId = "user@test.com" });

        var service = new OrderService(
            orderRepository.Object,
            bookRepository.Object,
            userRepository.Object,
            addressRepository.Object,
            genreRepository.Object,
            reviewRepository.Object,
            bookService.Object);

        var result = await service.CheckoutAsync(new CheckoutViewModel
        {
            AddressId = 10,
            PaymentMethod = "Kapıda ödeme",
            CartItems = new List<CartItemViewModel>
            {
                new() { BookId = 1, Quantity = 1 }
            }
        }, "user@test.com");

        Assert.False(result.Success);
        Assert.Contains("stok yetersiz", result.Error);
        orderRepository.Verify(r => r.Add(It.IsAny<Order>()), Times.Never);
        orderRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}
