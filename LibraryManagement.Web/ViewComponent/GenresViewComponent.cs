using System.Threading.Tasks;
using LibraryManagement.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Web.ViewComponents
{
    public class GenresViewComponent : ViewComponent
    {
        private readonly IGenreService _genreService;

        public GenresViewComponent(IGenreService genreService)
        {
            _genreService = genreService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var selectedGenreId = RouteData.Values["id"] as string;
            ViewData["SelectedGenre"] = selectedGenreId;

            var genres = await _genreService.GetAllOrderedAsync();
            return View(genres);
        }
    }
}
