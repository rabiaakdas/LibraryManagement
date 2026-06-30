using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagement.Web.Entity;

namespace LibraryManagement.Web.Services
{
    /// <summary>
    /// Defines book suggestion operations shown on the public suggestion flow.
    /// </summary>
    public interface ISuggestionService
    {
        Task<List<BookSuggestion>> GetSuggestionsAsync();
        Task AddSuggestionAsync(BookSuggestion suggestion);
        Task LikeAsync(int id);
    }
}
