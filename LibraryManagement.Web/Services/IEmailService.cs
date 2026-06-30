using System.Threading.Tasks;

namespace LibraryManagement.Web.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string body);
    }
}
