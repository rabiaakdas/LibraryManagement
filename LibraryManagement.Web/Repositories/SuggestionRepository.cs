using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Web.Data;
using LibraryManagement.Web.Entity;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Repositories
{
    public class SuggestionRepository : ISuggestionRepository
    {
        private readonly BookContext _context;

        public SuggestionRepository(BookContext context)
        {
            _context = context;
        }

        public async Task<List<BookSuggestion>> GetAllOrderedAsync()
        {
            return await _context.BookSuggestions
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<BookSuggestion> GetByIdAsync(int id)
        {
            return await _context.BookSuggestions.FindAsync(id);
        }

        public void Add(BookSuggestion suggestion)
        {
            _context.BookSuggestions.Add(suggestion);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
