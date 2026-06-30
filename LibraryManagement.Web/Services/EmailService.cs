using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using LibraryManagement.Web.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LibraryManagement.Web.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                if (_settings.UseFakeEmailSender)
                {
                    _logger.LogInformation(
                        "Fake email gönderildi. To: {ToEmail}, Subject: {Subject}, Body: {Body}",
                        toEmail,
                        subject,
                        body);

                    return true;
                }

                using var message = new MailMessage
                {
                    From = new MailAddress(_settings.FromEmail, _settings.FromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false
                };
                message.To.Add(toEmail);

                using var client = new SmtpClient(_settings.Host, _settings.Port)
                {
                    EnableSsl = _settings.EnableSsl,
                    Credentials = new NetworkCredential(_settings.UserName, _settings.Password)
                };

                await client.SendMailAsync(message);
                _logger.LogInformation("SMTP email gönderildi. To: {ToEmail}, Subject: {Subject}", toEmail, subject);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email gönderimi başarısız. To: {ToEmail}, Subject: {Subject}", toEmail, subject);
                return false;
            }
        }
    }
}
