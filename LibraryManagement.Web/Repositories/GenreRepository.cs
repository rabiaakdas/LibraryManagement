using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Web.Data;
using LibraryManagement.Web.Entity;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly BookContext _context;

        public GenreRepository(BookContext context)
        {
            _context = context;
        }

        public IQueryable<Genre> Query()
        {
            return _context.Genres.AsQueryable();
        }

        public IQueryable<Genre> QueryWithBooks()
        {
            return _context.Genres.Include(g => g.Books).AsQueryable();
        }

        public async Task<List<Genre>> GetAllOrderedAsync()
        {
            return await _context.Genres.OrderBy(g => g.Name).ToListAsync();
        }

        public async Task<List<Genre>> GetByIdsAsync(List<int> ids)
        {
            return await _context.Genres.Where(g => ids.Contains(g.GenreId)).ToListAsync();
        }

        public async Task<Genre> GetByIdAsync(int id)
        {
            return await _context.Genres.FindAsync(id);
        }

        public async Task<Genre> GetByIdWithBooksAsync(int id)
        {
            return await _context.Genres
                .Include(g => g.Books)
                .FirstOrDefaultAsync(g => g.GenreId == id);
        }

        public async Task<bool> NameExistsAsync(string name, int? excludedGenreId = null)
        {
            var normalizedName = name?.Trim().ToLower();
            return await _context.Genres.AnyAsync(g =>
                g.Name.ToLower() == normalizedName &&
                (!excludedGenreId.HasValue || g.GenreId != excludedGenreId.Value));
        }

        public void Add(Genre genre)
        {
            _context.Genres.Add(genre);
        }

        public void Remove(Genre genre)
        {
            _context.Genres.Remove(genre);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
