

using LibraryManagement.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LibraryManagement.Web.ViewComponents
{
    public class GenresViewComponent : ViewComponent
    {
        private readonly BookContext _context;

        public GenresViewComponent(BookContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // RouteData'dan seçili genre id'sini al
            var selectedGenreId = RouteData.Values["id"] as string;
            ViewData["SelectedGenre"] = selectedGenreId;

            // Tüm genre'ları asenkron çek
            var genres = await _context.Genres.ToListAsync();

            return View(genres);
        }
    }
}
