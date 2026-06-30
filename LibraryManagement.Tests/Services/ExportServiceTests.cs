using System.Text;
using LibraryManagement.Tests.Helpers;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Repositories;
using LibraryManagement.Web.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace LibraryManagement.Tests.Services;

public class ExportServiceTests
{
    private static ExportService CreateService(List<Order>? orders = null, List<Book>? books = null)
    {
        var orderRepository = new Mock<IOrderRepository>();
        var bookRepository = new Mock<IBookRepository>();
        var bookService = new Mock<IBookService>();
        var logger = new Mock<ILogger<ExportService>>();

        orderRepository.Setup(r => r.QueryWithItems()).Returns(AsyncQueryable.Create(orders ?? new List<Order>()));
        bookRepository.Setup(r => r.QueryWithGenres()).Returns(AsyncQueryable.Create(books ?? new List<Book>()));
        bookService.Setup(s => s.GetStockStatus(It.IsAny<int>())).Returns((int stock) =>
            stock <= 0 ? "OutOfStock" : stock <= 5 ? "LowStock" : "InStock");

        return new ExportService(orderRepository.Object, bookRepository.Object, bookService.Object, logger.Object);
    }

    [Fact]
    public async Task CreateOrdersReportAsync_ReturnsExcelBytes()
    {
        var service = CreateService(orders: new List<Order>
        {
            new()
            {
                Id = 1,
                UserId = "user@test.com",
                OrderDate = DateTime.Now,
                Status = "Pending",
                TotalAmount = 100,
                ShippingFee = 49.90m,
                DiscountAmount = 10,
                GrandTotal = 139.90m,
                PaymentMethod = "Kapıda ödeme",
                CouponCode = "TEST10",
                CargoCompany = "Test Kargo",
                TrackingNumber = "TRK1"
            }
        });

        var result = await service.CreateOrdersReportAsync();

        Assert.NotEmpty(result);
        Assert.Equal("PK", Encoding.ASCII.GetString(result.Take(2).ToArray()));
    }

    [Fact]
    public async Task CreateStockReportAsync_ReturnsExcelBytes()
    {
        var service = CreateService(books: new List<Book>
        {
            new()
            {
                BookId = 1,
                Title = "Dune",
                Author = "Frank Herbert",
                Price = 100,
                Stock = 3,
                Genres = new HashSet<Genre> { new() { GenreId = 1, Name = "Bilim Kurgu" } }
            }
        });

        var result = await service.CreateStockReportAsync();

        Assert.NotEmpty(result);
        Assert.Equal("PK", Encoding.ASCII.GetString(result.Take(2).ToArray()));
    }
}
