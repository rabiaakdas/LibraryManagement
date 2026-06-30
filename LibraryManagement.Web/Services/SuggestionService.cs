using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Repositories;

namespace LibraryManagement.Web.Services
{
    /// <summary>
    /// Provides book suggestion operations for the public suggestion feature.
    /// </summary>
    public class SuggestionService : ISuggestionService
    {
        private readonly ISuggestionRepository _suggestions;

        public SuggestionService(ISuggestionRepository suggestions)
        {
            _suggestions = suggestions;
        }

        public async Task<List<BookSuggestion>> GetSuggestionsAsync()
        {
            return await _suggestions.GetAllOrderedAsync();
        }

        public async Task AddSuggestionAsync(BookSuggestion suggestion)
        {
            _suggestions.Add(suggestion);
            await _suggestions.SaveChangesAsync();
        }

        public async Task LikeAsync(int id)
        {
            var suggestion = await _suggestions.GetByIdAsync(id);
            if (suggestion != null)
            {
                suggestion.Likes += 1;
                await _suggestions.SaveChangesAsync();
            }
        }
    }
}
