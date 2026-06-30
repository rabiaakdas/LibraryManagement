using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagement.Web.Areas.Admin.Models;
using LibraryManagement.Web.Models;
using LibraryManagement.Web.Models.Api;

namespace LibraryManagement.Web.Services
{
    /// <summary>
    /// Defines order operations for checkout, user order history, admin order management, and API endpoints.
    /// </summary>
    public interface IOrderService
    {
        Task<(bool Success, string Error)> CheckoutAsync(List<CartItemViewModel> cart, string userId);
        Task<(bool Success, string Error)> CheckoutAsync(CheckoutViewModel model, string userId);
        Task<List<OrderViewModel>> GetUserOrdersAsync(string userId);
        Task<AdminOrderListViewModel> GetAdminOrderListAsync();
        Task<AdminOrderDetailsViewModel> GetAdminOrderDetailsAsync(int id);
        Task<AdminOrderStatusViewModel> GetEditStatusModelAsync(int id);
        Task<(bool Success, string Error)> UpdateStatusAsync(int id, AdminOrderStatusViewModel model);
        Task<AdminDashboardViewModel> GetDashboardAsync();
        Task<List<OrderDto>> GetApiOrdersAsync();
        Task<OrderDto> GetApiOrderAsync(int id);
    }
}
