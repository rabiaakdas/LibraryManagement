using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Web.Entity;

namespace LibraryManagement.Web.Repositories
{
    public interface IBookRepository
    {
        IQueryable<Book> Query();
        IQueryable<Book> QueryWithGenres();
        Task<List<Book>> GetAllAsync();
        Task<Book> GetByIdAsync(int id);
        Task<Book> GetByIdWithGenresAsync(int id);
        Task<List<Book>> GetLowStockBooksAsync(int stockLimit);
        void Add(Book book);
        void Remove(Book book);
        Task SaveChangesAsync();
    }
}
