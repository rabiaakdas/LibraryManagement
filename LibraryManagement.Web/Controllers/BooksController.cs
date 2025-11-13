using LibraryManagement.Web.Data;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Web.Controllers
{
    public class BooksController : Controller
    {
       
        private readonly BookContext _context;
        public BooksController(BookContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> Index()
        {
            var model = new HomePageViewModel
            {
                PopularBooks = await _context.Books.ToListAsync() // Async çağrı
            };

            return View(model);
        }
       
        public async Task<IActionResult> List(int? id, string? q)
        {
            
            var booksQuery = _context.Books.AsQueryable();

            
            if (id.HasValue)
            {
                booksQuery = booksQuery
                    .Where(b => b.Genres.Any(g => g.GenreId == id.Value));
            }

            // Arama filtresi
            if (!string.IsNullOrWhiteSpace(q))
            {
                var searchTerm = q.ToLower();
                booksQuery = booksQuery
                    .Where(b => b.Title.ToLower().Contains(searchTerm));
            }

            // Verileri asenkron şekilde al
            var model = new BooksViewModel
            {
                Books = await booksQuery
                    .Include(b => b.Genres) // navigation property yükleme
                    .ToListAsync()
            };

            return View(model);
        }

        public IActionResult Details(int id)
        {
             return View(_context.Books.Find(id));
        }

        [HttpPost]
        public IActionResult AddToFavorites(int id)
        {
            var userId = User.Identity.Name;
            if (userId == null)
            {
                return Json(new { success = false, message = "Giriş yapmanız gerekiyor." });
            }

            var alreadyFav = _context.Favorites
                .FirstOrDefault(f => f.BookId == id && f.UserId == userId);

            if (alreadyFav == null)
            {
                var fav = new Favorite
                {
                    UserId = userId,
                    BookId = id
                };
                _context.Favorites.Add(fav);
                _context.SaveChanges();
            }

            return Json(new { success = true, message = "Favorilere eklendi." });
        }


    }
}
