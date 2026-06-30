using System.Threading.Tasks;
using LibraryManagement.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBookService _bookService;

        public HomeController(IBookService bookService)
        {
            _bookService = bookService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _bookService.GetHomePageAsync());
        }

        public IActionResult Error()
        {
            Response.StatusCode = 500;
            return View();
        }
    }
}
