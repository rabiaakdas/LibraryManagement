using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Models;
using LibraryManagement.Web.Repositories;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LibraryManagement.Web.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<InvoiceService> _logger;

        public InvoiceService(IOrderRepository orderRepository, ILogger<InvoiceService> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<InvoiceResultViewModel> CreateInvoiceAsync(int orderId, string currentUserId, bool isAdmin = false)
        {
            try
            {
                var order = await _orderRepository.GetByIdWithItemsAsync(orderId);
                if (order == null)
                {
                    return new InvoiceResultViewModel
                    {
                        NotFound = true,
                        Error = "Siparis bulunamadi."
                    };
                }

                if (!isAdmin && !string.Equals(order.UserId, currentUserId, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning(
                        "Yetkisiz fatura erisim denemesi. OrderId: {OrderId}, CurrentUserId: {CurrentUserId}, OrderUserId: {OrderUserId}",
                        order.Id,
                        currentUserId,
                        order.UserId);

                    return new InvoiceResultViewModel
                    {
                        Forbidden = true,
                        Error = "Bu faturaya erisim yetkiniz yok."
                    };
                }

                var pdfBytes = CreatePdf(order);
                _logger.LogInformation("Fatura olusturuldu. OrderId: {OrderId}, UserId: {UserId}", order.Id, order.UserId);

                return new InvoiceResultViewModel
                {
                    Success = true,
                    PdfBytes = pdfBytes,
                    FileName = $"fatura-{order.Id}.pdf"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fatura olusturma hatasi. OrderId: {OrderId}", orderId);

                return new InvoiceResultViewModel
                {
                    Error = "Fatura olusturulurken bir hata olustu."
                };
            }
        }

        private static byte[] CreatePdf(Order order)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var culture = new CultureInfo("tr-TR");
            var grandTotal = order.GrandTotal == 0 ? order.TotalAmount : order.GrandTotal;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Column(column =>
                    {
                        column.Item().Text("Library Management Fatura").FontSize(20).Bold();
                        column.Item().Text($"Olusturma Tarihi: {DateTime.Now:dd.MM.yyyy HH:mm}").FontSize(9).FontColor(Colors.Grey.Darken2);
                    });

                    page.Content().PaddingTop(20).Column(column =>
                    {
                        column.Spacing(14);

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(info =>
                            {
                                info.Item().Text("Siparis Bilgileri").Bold();
                                info.Item().Text($"Siparis No: {order.Id}");
                                info.Item().Text($"Siparis Tarihi: {order.OrderDate:dd.MM.yyyy HH:mm}");
                                info.Item().Text($"Kullanici: {order.UserId}");
                                info.Item().Text($"Odeme Yontemi: {DisplayText(order.PaymentMethod)}");
                                info.Item().Text($"Siparis Durumu: {DisplayText(order.Status)}");
                            });

                            row.RelativeItem().Column(info =>
                            {
                                info.Item().Text("Teslimat Adresi").Bold();
                                info.Item().Text(GetAddressText(order.Address));

                                if (!string.IsNullOrWhiteSpace(order.CargoCompany) || !string.IsNullOrWhiteSpace(order.TrackingNumber))
                                {
                                    info.Item().PaddingTop(8).Text("Kargo Bilgileri").Bold();
                                    info.Item().Text($"Kargo Sirketi: {DisplayText(order.CargoCompany)}");
                                    info.Item().Text($"Takip No: {DisplayText(order.TrackingNumber)}");
                                }
                            });
                        });

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(4);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderCell).Text("Kitap Adi");
                                header.Cell().Element(HeaderCell).AlignRight().Text("Adet");
                                header.Cell().Element(HeaderCell).AlignRight().Text("Birim Fiyat");
                                header.Cell().Element(HeaderCell).AlignRight().Text("Toplam");
                            });

                            foreach (var item in order.Items)
                            {
                                table.Cell().Element(BodyCell).Text(item.Title);
                                table.Cell().Element(BodyCell).AlignRight().Text(item.Quantity.ToString());
                                table.Cell().Element(BodyCell).AlignRight().Text(item.Price.ToString("C", culture));
                                table.Cell().Element(BodyCell).AlignRight().Text(item.TotalPrice.ToString("C", culture));
                            }
                        });

                        column.Item().AlignRight().Width(230).Column(totals =>
                        {
                            totals.Item().Row(row =>
                            {
                                row.RelativeItem().Text("Ara Toplam");
                                row.ConstantItem(100).AlignRight().Text(order.TotalAmount.ToString("C", culture));
                            });
                            totals.Item().Row(row =>
                            {
                                row.RelativeItem().Text("Kargo Ucreti");
                                row.ConstantItem(100).AlignRight().Text(order.ShippingFee == 0 ? "Ucretsiz" : order.ShippingFee.ToString("C", culture));
                            });

                            if (!string.IsNullOrWhiteSpace(order.CouponCode))
                            {
                                totals.Item().Row(row =>
                                {
                                    row.RelativeItem().Text("Kupon Kodu");
                                    row.ConstantItem(100).AlignRight().Text(order.CouponCode);
                                });
                            }

                            if (order.DiscountAmount > 0)
                            {
                                totals.Item().Row(row =>
                                {
                                    row.RelativeItem().Text("Indirim");
                                    row.ConstantItem(100).AlignRight().Text("-" + order.DiscountAmount.ToString("C", culture));
                                });
                            }

                            totals.Item().PaddingTop(6).BorderTop(1).BorderColor(Colors.Grey.Lighten1).Row(row =>
                            {
                                row.RelativeItem().Text("Genel Toplam").Bold();
                                row.ConstantItem(100).AlignRight().Text(grandTotal.ToString("C", culture)).Bold();
                            });
                        });
                    });

                    page.Footer().AlignCenter().Text("Bu belge proje ici ornek PDF faturadir, resmi e-fatura yerine gecmez.").FontSize(9);
                });
            }).GeneratePdf();
        }

        private static string GetAddressText(Address address)
        {
            if (address == null)
            {
                return "Adres bilgisi yok";
            }

            return $"{DisplayText(address.Title)}\n{DisplayText(address.FullAddress)}\n{DisplayText(address.District)} / {DisplayText(address.City)} {DisplayText(address.ZipCode)}";
        }

        private static string DisplayText(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "-" : value;
        }

        private static IContainer HeaderCell(IContainer container)
        {
            return container.Background(Colors.Grey.Lighten3).Padding(5).DefaultTextStyle(x => x.Bold());
        }

        private static IContainer BodyCell(IContainer container)
        {
            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5);
        }
    }
}
