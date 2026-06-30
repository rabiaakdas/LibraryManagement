using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Web.Data;
using LibraryManagement.Web.Entity;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly BookContext _context;

        public ReviewRepository(BookContext context)
        {
            _context = context;
        }

        public IQueryable<BookReview> Query()
        {
            return _context.BookReviews.AsQueryable();
        }

        public IQueryable<BookReview> QueryWithBookAndUser()
        {
            return _context.BookReviews
                .Include(r => r.Book)
                .Include(r => r.User)
                .AsQueryable();
        }

        public IQueryable<BookReview> QueryWithUser()
        {
            return _context.BookReviews.Include(r => r.User).AsQueryable();
        }

        public async Task<BookReview> GetByIdAsync(int id)
        {
            return await _context.BookReviews.FindAsync(id);
        }

        public async Task<BookReview> GetByIdWithBookAndUserAsync(int id)
        {
            return await _context.BookReviews
                .Include(r => r.Book)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<bool> ExistsForBookAndUserAsync(int bookId, int userId)
        {
            return await _context.BookReviews.AnyAsync(r => r.BookId == bookId && r.UserId == userId);
        }

        public void Add(BookReview review)
        {
            _context.BookReviews.Add(review);
        }

        public void Remove(BookReview review)
        {
            _context.BookReviews.Remove(review);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
