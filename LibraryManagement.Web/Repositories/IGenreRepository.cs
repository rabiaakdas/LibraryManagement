using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Web.Entity;

namespace LibraryManagement.Web.Repositories
{
    public interface IGenreRepository
    {
        IQueryable<Genre> Query();
        IQueryable<Genre> QueryWithBooks();
        Task<List<Genre>> GetAllOrderedAsync();
        Task<List<Genre>> GetByIdsAsync(List<int> ids);
        Task<Genre> GetByIdAsync(int id);
        Task<Genre> GetByIdWithBooksAsync(int id);
        Task<bool> NameExistsAsync(string name, int? excludedGenreId = null);
        void Add(Genre genre);
        void Remove(Genre genre);
        Task SaveChangesAsync();
    }
}
