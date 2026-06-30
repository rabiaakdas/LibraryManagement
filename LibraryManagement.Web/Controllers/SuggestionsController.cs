using System.Threading.Tasks;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Web.Controllers
{
    public class SuggestionsController : Controller
    {
        private readonly ISuggestionService _suggestionService;

        public SuggestionsController(ISuggestionService suggestionService)
        {
            _suggestionService = suggestionService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _suggestionService.GetSuggestionsAsync());
        }

        [HttpPost]
        public async Task<IActionResult> AddSuggestion(BookSuggestion suggestion)
        {
            if (ModelState.IsValid)
            {
                await _suggestionService.AddSuggestionAsync(suggestion);
                TempData["Message"] = "Öneriniz kaydedildi!";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Bir hata oluştu, lütfen tekrar deneyin.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Like(int id)
        {
            await _suggestionService.LikeAsync(id);
            return RedirectToAction("Index");
        }
    }
}
