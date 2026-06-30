using System.Threading.Tasks;
using LibraryManagement.Web.Areas.Admin.Models;
using LibraryManagement.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class GenresController : Controller
    {
        private readonly IGenreService _genreService;

        public GenresController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _genreService.GetAdminGenreListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var genre = await _genreService.GetGenreWithBooksAsync(id);
            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new AdminGenreFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminGenreFormViewModel model)
        {
            if (await _genreService.NameExistsAsync(model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Bu isimde bir kategori zaten var.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _genreService.CreateGenreAsync(model);
            TempData["Message"] = "Kategori basariyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _genreService.GetEditGenreModelAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdminGenreFormViewModel model)
        {
            if (id != model.GenreId)
            {
                return NotFound();
            }

            if (await _genreService.NameExistsAsync(model.Name, model.GenreId))
            {
                ModelState.AddModelError(nameof(model.Name), "Bu isimde bir kategori zaten var.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var updated = await _genreService.UpdateGenreAsync(id, model);
            if (!updated)
            {
                return NotFound();
            }

            TempData["Message"] = "Kategori basariyla guncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var genre = await _genreService.GetGenreWithBooksAsync(id);
            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _genreService.DeleteGenreAsync(id);
            if (result.Success)
            {
                TempData["Message"] = "Kategori basariyla silindi.";
            }
            else
            {
                TempData["Error"] = result.Error;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
