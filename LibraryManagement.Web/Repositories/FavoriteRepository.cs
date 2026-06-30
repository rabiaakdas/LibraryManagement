using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Web.Data;
using LibraryManagement.Web.Entity;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Repositories
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly BookContext _context;

        public FavoriteRepository(BookContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(int bookId, string userId)
        {
            return await _context.Favorites.AnyAsync(f => f.BookId == bookId && f.UserId == userId);
        }

        public async Task<List<Favorite>> GetByUserWithBookAsync(string userId)
        {
            return await _context.Favorites
                .Where(f => f.UserId == userId)
                .Include(f => f.Book)
                .ToListAsync();
        }

        public void Add(Favorite favorite)
        {
            _context.Favorites.Add(favorite);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
