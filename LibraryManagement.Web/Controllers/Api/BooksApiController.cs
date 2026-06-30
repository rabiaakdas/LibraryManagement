using System.Threading.Tasks;
using LibraryManagement.Web.Models.Api;
using LibraryManagement.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Web.Controllers.Api
{
    [ApiController]
    [Route("api/books")]
    public class BooksApiController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksApiController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _bookService.GetApiBooksAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var book = await _bookService.GetApiBookAsync(id);
            return book == null ? NotFound() : Ok(book);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Post(BookUpsertDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var book = await _bookService.CreateApiBookAsync(model);
            return CreatedAtAction(nameof(Get), new { id = book.Id }, book);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, BookUpsertDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _bookService.UpdateApiBookAsync(id, model);
            return result.Success ? Ok(result.Book) : NotFound();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingBook = await _bookService.GetApiBookAsync(id);
            if (existingBook == null)
            {
                return NotFound();
            }

            var result = await _bookService.DeleteBookAsync(id);
            return result.Success ? Ok() : BadRequest(result.Error);
        }
    }
}
