using System.Threading.Tasks;

namespace LibraryManagement.Web.Services
{
    public interface IExportService
    {
        Task<byte[]> CreateOrdersReportAsync();
        Task<byte[]> CreateStockReportAsync();
    }
}
