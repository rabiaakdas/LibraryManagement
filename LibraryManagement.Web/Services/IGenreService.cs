using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagement.Web.Areas.Admin.Models;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Models.Api;

namespace LibraryManagement.Web.Services
{
    /// <summary>
    /// Defines genre operations for category pages, admin management, and API endpoints.
    /// </summary>
    public interface IGenreService
    {
        Task<List<Genre>> GetAllOrderedAsync();
        Task<AdminGenreListViewModel> GetAdminGenreListAsync();
        Task<Genre> GetGenreWithBooksAsync(int id);
        Task<AdminGenreFormViewModel> GetEditGenreModelAsync(int id);
        Task<bool> NameExistsAsync(string name, int? excludedGenreId = null);
        Task CreateGenreAsync(AdminGenreFormViewModel model);
        Task<bool> UpdateGenreAsync(int id, AdminGenreFormViewModel model);
        Task<(bool Success, string Error)> DeleteGenreAsync(int id);
        Task<List<GenreDto>> GetApiGenresAsync();
        Task<GenreDto> GetApiGenreAsync(int id);
    }
}
