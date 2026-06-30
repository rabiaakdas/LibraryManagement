using System.Threading.Tasks;
using LibraryManagement.Web.Areas.Admin.Models;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Models;

namespace LibraryManagement.Web.Services
{
    /// <summary>
    /// Defines review operations for book comments and admin review moderation.
    /// </summary>
    public interface IReviewService
    {
        Task<bool> AddReviewAsync(BookReviewCreateViewModel model, int userId);
        Task<AdminReviewFilterViewModel> GetAdminReviewsAsync(AdminReviewFilterViewModel filter);
        Task<BookReview> GetReviewDetailsAsync(int id);
        Task<(bool Success, string Error)> DeleteReviewAsync(int id);
    }
}
