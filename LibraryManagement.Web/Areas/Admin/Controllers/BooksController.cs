using System.Threading.Tasks;
using LibraryManagement.Web.Areas.Admin.Models;
using LibraryManagement.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        public async Task<IActionResult> Index(string stockFilter)
        {
            return View(await _bookService.GetAdminBookListAsync(stockFilter));
        }

        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookService.GetBookWithGenresAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View(await _bookService.GetCreateBookModelAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminBookFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var filledModel = await _bookService.GetCreateBookModelAsync();
                filledModel.Title = model.Title;
                filledModel.Author = model.Author;
                filledModel.ImageUrl = model.ImageUrl;
                filledModel.PageCount = model.PageCount;
                filledModel.Price = model.Price;
                filledModel.Stock = model.Stock;
                filledModel.SelectedGenreIds = model.SelectedGenreIds;
                return View(filledModel);
            }

            await _bookService.CreateBookAsync(model);
            TempData["Message"] = "Kitap basariyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _bookService.GetEditBookModelAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdminBookFormViewModel model)
        {
            if (id != model.BookId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var filledModel = await _bookService.GetEditBookModelAsync(id);
                if (filledModel == null)
                {
                    return NotFound();
                }
                filledModel.Title = model.Title;
                filledModel.Author = model.Author;
                filledModel.ImageUrl = model.ImageUrl;
                filledModel.PageCount = model.PageCount;
                filledModel.Price = model.Price;
                filledModel.Stock = model.Stock;
                filledModel.SelectedGenreIds = model.SelectedGenreIds;
                return View(filledModel);
            }

            var updated = await _bookService.UpdateBookAsync(id, model);
            if (!updated)
            {
                return NotFound();
            }

            TempData["Message"] = "Kitap basariyla guncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookService.GetBookWithGenresAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _bookService.DeleteBookAsync(id);
            if (result.Success)
            {
                TempData["Message"] = "Kitap basariyla silindi.";
            }
            else
            {
                TempData["Error"] = result.Error;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
