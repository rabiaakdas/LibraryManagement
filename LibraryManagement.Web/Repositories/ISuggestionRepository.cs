using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagement.Web.Entity;

namespace LibraryManagement.Web.Repositories
{
    public interface ISuggestionRepository
    {
        Task<List<BookSuggestion>> GetAllOrderedAsync();
        Task<BookSuggestion> GetByIdAsync(int id);
        void Add(BookSuggestion suggestion);
        Task SaveChangesAsync();
    }
}
