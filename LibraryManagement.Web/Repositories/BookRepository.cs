using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Web.Data;
using LibraryManagement.Web.Entity;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly BookContext _context;

        public BookRepository(BookContext context)
        {
            _context = context;
        }

        public IQueryable<Book> Query()
        {
            return _context.Books.AsQueryable();
        }

        public IQueryable<Book> QueryWithGenres()
        {
            return _context.Books.Include(b => b.Genres).AsQueryable();
        }

        public async Task<List<Book>> GetAllAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<Book> GetByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task<Book> GetByIdWithGenresAsync(int id)
        {
            return await _context.Books
                .Include(b => b.Genres)
                .FirstOrDefaultAsync(b => b.BookId == id);
        }

        public async Task<List<Book>> GetLowStockBooksAsync(int stockLimit)
        {
            return await _context.Books
                .Where(b => b.Stock <= stockLimit)
                .OrderBy(b => b.Stock)
                .ThenBy(b => b.Title)
                .ToListAsync();
        }

        public void Add(Book book)
        {
            _context.Books.Add(book);
        }

        public void Remove(Book book)
        {
            _context.Books.Remove(book);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
