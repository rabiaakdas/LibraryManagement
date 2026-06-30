using System.Threading.Tasks;
using LibraryManagement.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Web.Controllers.Api
{
    [ApiController]
    [Route("api/genres")]
    public class GenresApiController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenresApiController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _genreService.GetApiGenresAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var genre = await _genreService.GetApiGenreAsync(id);
            return genre == null ? NotFound() : Ok(genre);
        }
    }
}
