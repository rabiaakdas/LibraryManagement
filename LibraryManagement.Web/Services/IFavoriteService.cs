using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagement.Web.Entity;

namespace LibraryManagement.Web.Services
{
    /// <summary>
    /// Defines favorite book operations for authenticated users.
    /// </summary>
    public interface IFavoriteService
    {
        Task AddToFavoritesAsync(int bookId, string userId);
        Task<List<Favorite>> GetUserFavoritesAsync(string userId);
    }
}
