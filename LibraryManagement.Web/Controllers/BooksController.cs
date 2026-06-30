using System.Security.Claims;
using System.Threading.Tasks;
using LibraryManagement.Web.Models;
using LibraryManagement.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Web.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IReviewService _reviewService;
        private readonly IFavoriteService _favoriteService;
        private readonly IUserService _userService;

        public BooksController(
            IBookService bookService,
            IReviewService reviewService,
            IFavoriteService favoriteService,
            IUserService userService)
        {
            _bookService = bookService;
            _reviewService = reviewService;
            _favoriteService = favoriteService;
            _userService = userService;
        }

        public async Task<IActionResult> Index(BookFilterViewModel filter)
        {
            return View(await _bookService.GetFilteredBooksAsync(filter));
        }

        public IActionResult List(int? id, string q)
        {
            return RedirectToAction(nameof(Index), new { Search = q, CategoryId = id });
        }

        public async Task<IActionResult> Details(int id)
        {
            var model = await _bookService.GetBookDetailsAsync(
                id,
                await GetCurrentUserIdAsync(),
                User.Identity?.IsAuthenticated == true);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(BookReviewCreateViewModel model)
        {
            var userId = await GetCurrentUserIdAsync();
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Users");
            }

            var book = await _bookService.GetBookWithGenresAsync(model.BookId);
            if (book == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Yorum eklenemedi. Puan ve yorum alanlarini kontrol edin.";
                return RedirectToAction(nameof(Details), new { id = model.BookId });
            }

            var added = await _reviewService.AddReviewAsync(model, userId.Value);
            if (!added)
            {
                TempData["Error"] = "Bu kitap icin zaten yorum yaptiniz.";
                return RedirectToAction(nameof(Details), new { id = model.BookId });
            }

            TempData["Success"] = "Yorumunuz basariyla eklendi.";
            return RedirectToAction(nameof(Details), new { id = model.BookId });
        }

        [HttpPost]
        public async Task<IActionResult> AddToFavorites(int id)
        {
            var userId = User.Identity?.Name;
            if (userId == null)
            {
                return Json(new { success = false, message = "Giriş yapmanız gerekiyor." });
            }

            await _favoriteService.AddToFavoritesAsync(id, userId);
            return Json(new { success = true, message = "Favorilere eklendi." });
        }

        private async Task<int?> GetCurrentUserIdAsync()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            var email = User.Identity?.Name;
            return string.IsNullOrEmpty(email) ? null : await _userService.GetUserIdByEmailAsync(email);
        }
    }
}
