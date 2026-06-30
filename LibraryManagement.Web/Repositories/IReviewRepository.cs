using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Web.Entity;

namespace LibraryManagement.Web.Repositories
{
    public interface IReviewRepository
    {
        IQueryable<BookReview> Query();
        IQueryable<BookReview> QueryWithBookAndUser();
        IQueryable<BookReview> QueryWithUser();
        Task<BookReview> GetByIdAsync(int id);
        Task<BookReview> GetByIdWithBookAndUserAsync(int id);
        Task<bool> ExistsForBookAndUserAsync(int bookId, int userId);
        void Add(BookReview review);
        void Remove(BookReview review);
        Task SaveChangesAsync();
    }
}
