using LibraryManagement.Web.Models;
using LibraryManagement.Web.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace LibraryManagement.Tests.Services;

public class EmailServiceTests
{
    [Fact]
    public async Task SendEmailAsync_WhenFakeSenderEnabled_ReturnsTrue()
    {
        var settings = Options.Create(new EmailSettings
        {
            UseFakeEmailSender = true,
            FromEmail = "noreply@example.com",
            FromName = "LibraryManagement"
        });
        var logger = new Mock<ILogger<EmailService>>();
        var service = new EmailService(settings, logger.Object);

        var result = await service.SendEmailAsync("user@test.com", "Test", "Body");

        Assert.True(result);
    }
}
