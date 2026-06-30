using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagement.Web.Areas.Admin.Models;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Models;
using LibraryManagement.Web.Models.Api;

namespace LibraryManagement.Web.Services
{
    /// <summary>
    /// Defines book operations for MVC pages, admin screens, dashboard widgets, and API endpoints.
    /// </summary>
    public interface IBookService
    {
        Task<HomePageViewModel> GetHomePageAsync();
        Task<BookFilterViewModel> GetFilteredBooksAsync(BookFilterViewModel filter);
        Task<BookDetailsViewModel> GetBookDetailsAsync(int id, int? currentUserId, bool isAuthenticated);
        Task<AdminBookListViewModel> GetAdminBookListAsync(string stockFilter = null);
        string GetStockStatus(int stock);
        Task<Book> GetBookWithGenresAsync(int id);
        Task<AdminBookFormViewModel> GetCreateBookModelAsync();
        Task<AdminBookFormViewModel> GetEditBookModelAsync(int id);
        Task CreateBookAsync(AdminBookFormViewModel model);
        Task<bool> UpdateBookAsync(int id, AdminBookFormViewModel model);
        Task<(bool Success, string Error)> DeleteBookAsync(int id);
        Task<List<DashboardLowStockBookViewModel>> GetLowStockDashboardAsync();
        Task<List<BookDto>> GetApiBooksAsync();
        Task<BookDto> GetApiBookAsync(int id);
        Task<BookDto> CreateApiBookAsync(BookUpsertDto model);
        Task<(bool Success, BookDto Book)> UpdateApiBookAsync(int id, BookUpsertDto model);
    }
}
