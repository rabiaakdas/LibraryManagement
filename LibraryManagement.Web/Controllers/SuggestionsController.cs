using LibraryManagement.Web.Data;
using LibraryManagement.Web.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace LibraryManagement.Web.Controllers
{
    public class SuggestionsController : Controller
    {
        private readonly BookContext _context;

        public SuggestionsController(BookContext context)
        {
            _context = context;
        }

        // Sayfa: Liste + Form
        public IActionResult Index()
        {
            var suggestions = _context.BookSuggestions
                                      .OrderByDescending(s => s.CreatedAt)
                                      .ToList();
            return View(suggestions);
        }

        // Yeni öneri ekleme
        [HttpPost]
        public IActionResult AddSuggestion(BookSuggestion suggestion)
        {
            if (ModelState.IsValid)
            {
                _context.BookSuggestions.Add(suggestion);
                _context.SaveChanges();
                TempData["Message"] = "Öneriniz kaydedildi!";
                return RedirectToAction("Index");
            }
            TempData["Error"] = "Bir hata oluştu, lütfen tekrar deneyin.";
            return RedirectToAction("Index");
        }

        // Beğeni ekleme
        [HttpPost]
        public IActionResult Like(int id)
        {
            var suggestion = _context.BookSuggestions.Find(id);
            if (suggestion != null)
            {
                suggestion.Likes += 1;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
