
using LibraryManagement.Web.Data;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly BookContext _context;

        public HomeController(BookContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomePageViewModel
            {
                PopularBooks = await _context.Books.ToListAsync()
            };

            return View(model);
        }

       
    }
}
