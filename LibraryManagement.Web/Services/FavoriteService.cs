using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Repositories;

namespace LibraryManagement.Web.Services
{
    /// <summary>
    /// Provides favorite book operations for authenticated users.
    /// </summary>
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favorites;

        public FavoriteService(IFavoriteRepository favorites)
        {
            _favorites = favorites;
        }

        public async Task AddToFavoritesAsync(int bookId, string userId)
        {
            if (!await _favorites.ExistsAsync(bookId, userId))
            {
                _favorites.Add(new Favorite { BookId = bookId, UserId = userId });
                await _favorites.SaveChangesAsync();
            }
        }

        public async Task<List<Favorite>> GetUserFavoritesAsync(string userId)
        {
            return await _favorites.GetByUserWithBookAsync(userId);
        }
    }
}
