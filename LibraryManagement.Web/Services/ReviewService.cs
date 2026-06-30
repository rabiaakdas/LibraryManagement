using System;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Web.Areas.Admin.Models;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Models;
using LibraryManagement.Web.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Web.Services
{
    /// <summary>
    /// Provides review business logic for book comments and admin moderation.
    /// </summary>
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviews;
        private readonly IBookRepository _books;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(
            IReviewRepository reviews,
            IBookRepository books,
            ILogger<ReviewService> logger = null)
        {
            _reviews = reviews;
            _books = books;
            _logger = logger;
        }

        public async Task<bool> AddReviewAsync(BookReviewCreateViewModel model, int userId)
        {
            var bookExists = await _books.Query().AnyAsync(b => b.BookId == model.BookId);
            if (!bookExists || await _reviews.ExistsForBookAndUserAsync(model.BookId, userId))
            {
                return false;
            }

            var review = new BookReview
            {
                BookId = model.BookId,
                UserId = userId,
                Rating = model.Rating,
                Comment = model.Comment
            };

            _reviews.Add(review);

            await _reviews.SaveChangesAsync();

            _logger?.LogInformation(
                "Yorum eklendi. ReviewId: {ReviewId}, BookId: {BookId}, UserId: {UserId}, Rating: {Rating}",
                review.Id,
                review.BookId,
                review.UserId,
                review.Rating);

            return true;
        }

        public async Task<AdminReviewFilterViewModel> GetAdminReviewsAsync(AdminReviewFilterViewModel filter)
        {
            var reviewsQuery = _reviews.QueryWithBookAndUser();

            if (!string.IsNullOrWhiteSpace(filter.BookSearch))
            {
                var bookSearch = filter.BookSearch.ToLower();
                reviewsQuery = reviewsQuery.Where(r => r.Book.Title.ToLower().Contains(bookSearch));
            }

            if (!string.IsNullOrWhiteSpace(filter.UserSearch))
            {
                var userSearch = filter.UserSearch.ToLower();
                reviewsQuery = reviewsQuery.Where(r =>
                    r.User.Username.ToLower().Contains(userSearch) ||
                    r.User.Email.ToLower().Contains(userSearch));
            }

            if (filter.Rating.HasValue)
            {
                reviewsQuery = reviewsQuery.Where(r => r.Rating == filter.Rating.Value);
            }

            reviewsQuery = filter.Sort == "oldest"
                ? reviewsQuery.OrderBy(r => r.CreatedAt)
                : reviewsQuery.OrderByDescending(r => r.CreatedAt);

            filter.Reviews = await reviewsQuery.ToListAsync();
            return filter;
        }

        public async Task<BookReview> GetReviewDetailsAsync(int id)
        {
            return await _reviews.GetByIdWithBookAndUserAsync(id);
        }

        public async Task<(bool Success, string Error)> DeleteReviewAsync(int id)
        {
            var review = await _reviews.GetByIdAsync(id);
            if (review == null)
            {
                return (false, "Yorum bulunamadi.");
            }

            _reviews.Remove(review);
            await _reviews.SaveChangesAsync();

            _logger?.LogInformation(
                "Yorum silindi. ReviewId: {ReviewId}, BookId: {BookId}, UserId: {UserId}",
                review.Id,
                review.BookId,
                review.UserId);

            return (true, null);
        }
    }
}
