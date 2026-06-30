using System.Text;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Repositories;
using LibraryManagement.Web.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace LibraryManagement.Tests.Services;

public class InvoiceServiceTests
{
    private static InvoiceService CreateService(Mock<IOrderRepository> orderRepository)
    {
        var logger = new Mock<ILogger<InvoiceService>>();
        return new InvoiceService(orderRepository.Object, logger.Object);
    }

    [Fact]
    public async Task CreateInvoiceAsync_WhenOrderExists_ReturnsPdfBytes()
    {
        var orderRepository = new Mock<IOrderRepository>();
        orderRepository.Setup(r => r.GetByIdWithItemsAsync(1)).ReturnsAsync(new Order
        {
            Id = 1,
            UserId = "user@test.com",
            OrderDate = DateTime.Now,
            TotalAmount = 200,
            ShippingFee = 49.90m,
            DiscountAmount = 20,
            GrandTotal = 229.90m,
            PaymentMethod = "Kapıda ödeme",
            Status = "Pending",
            Address = new Address
            {
                Title = "Ev",
                City = "Istanbul",
                District = "Kadikoy",
                ZipCode = "34000",
                FullAddress = "Test adres"
            },
            Items = new List<OrderItem>
            {
                new() { Title = "Dune", Quantity = 2, Price = 100, TotalPrice = 200 }
            }
        });
        var service = CreateService(orderRepository);

        var result = await service.CreateInvoiceAsync(1, "user@test.com");

        Assert.True(result.Success);
        Assert.NotEmpty(result.PdfBytes);
        Assert.Equal("%PDF", Encoding.ASCII.GetString(result.PdfBytes.Take(4).ToArray()));
        Assert.Equal("fatura-1.pdf", result.FileName);
    }

    [Fact]
    public async Task CreateInvoiceAsync_WhenOrderDoesNotExist_ReturnsNotFound()
    {
        var orderRepository = new Mock<IOrderRepository>();
        orderRepository.Setup(r => r.GetByIdWithItemsAsync(99)).ReturnsAsync((Order?)null);
        var service = CreateService(orderRepository);

        var result = await service.CreateInvoiceAsync(99, "user@test.com");

        Assert.False(result.Success);
        Assert.True(result.NotFound);
        Assert.Empty(result.PdfBytes);
    }

    [Fact]
    public async Task CreateInvoiceAsync_WhenUserDoesNotOwnOrder_ReturnsForbidden()
    {
        var orderRepository = new Mock<IOrderRepository>();
        orderRepository.Setup(r => r.GetByIdWithItemsAsync(5)).ReturnsAsync(new Order
        {
            Id = 5,
            UserId = "owner@test.com",
            Items = new List<OrderItem>()
        });
        var service = CreateService(orderRepository);

        var result = await service.CreateInvoiceAsync(5, "other@test.com");

        Assert.False(result.Success);
        Assert.True(result.Forbidden);
        Assert.Empty(result.PdfBytes);
    }
}
