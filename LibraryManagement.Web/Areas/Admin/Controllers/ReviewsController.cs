using System.Threading.Tasks;
using LibraryManagement.Web.Areas.Admin.Models;
using LibraryManagement.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReviewsController : Controller
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<IActionResult> Index(AdminReviewFilterViewModel filter)
        {
            return View(await _reviewService.GetAdminReviewsAsync(filter));
        }

        public async Task<IActionResult> Details(int id)
        {
            var review = await _reviewService.GetReviewDetailsAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _reviewService.GetReviewDetailsAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _reviewService.DeleteReviewAsync(id);
            if (result.Success)
            {
                TempData["Success"] = "Yorum basariyla silindi.";
            }
            else
            {
                TempData["Error"] = result.Error;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
