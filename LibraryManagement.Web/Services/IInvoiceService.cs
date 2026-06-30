using System.Threading.Tasks;
using LibraryManagement.Web.Models;

namespace LibraryManagement.Web.Services
{
    public interface IInvoiceService
    {
        Task<InvoiceResultViewModel> CreateInvoiceAsync(int orderId, string currentUserId, bool isAdmin = false);
    }
}
