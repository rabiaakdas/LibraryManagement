using LibraryManagement.Web.Data;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LibraryManagement.Web.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly BookContext _context;

        public ProfilesController(BookContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Kullanıcının giriş bilgisi (örnek: email) Claims'ten alınır
            var email = User.Identity?.Name;

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Users"); // giriş yapılmamışsa login'e yönlendir
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileViewModel
            {
                FullName = user.Username,
                Email = user.Email,
                
            };

            return View(model);
        }

        public IActionResult Address()
        {
            var userId = User.Identity.Name;
            var adres = _context.Addresses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Id)
                .Select(a => new AddressViewModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    City = a.City,
                    District = a.District,
                    ZipCode = a.ZipCode,
                    FullAddress = a.FullAddress
                })
                .ToList();


            return View(adres);
        }


        public IActionResult CreateAddress()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAddress(AddressViewModel model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine("Validation Error: " + error.ErrorMessage);
                }
                TempData["Error"] = "Form geçersiz!";
                return View(model);
            }

            var address = new Address
            {
                Title = model.Title,
                City = model.City,
                District = model.District,
                ZipCode = model.ZipCode,
                FullAddress = model.FullAddress,
                UserId = User.Identity.Name
            };

            _context.Add(address);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Adres başarıyla kaydedildi.";
            return RedirectToAction("Address");
        }

        [HttpGet]
        public IActionResult DeleteAddress(int id)
        {
            var address = _context.Addresses.FirstOrDefault(a => a.Id == id);
            if (address == null)
                return NotFound();

            _context.Addresses.Remove(address);
            _context.SaveChanges();

            TempData["Message"] = "Adres başarıyla silindi.";
            return RedirectToAction("Address");
        }

        // GET: Formu aç
        [HttpGet]
        public IActionResult EditAddress(int id)
        {
            var address = _context.Addresses.FirstOrDefault(a => a.Id == id);
            if (address == null)
                return NotFound();

            var model = new AddressViewModel
            {
                Id = address.Id,
                Title = address.Title,
                City = address.City,
                District = address.District,
                ZipCode = address.ZipCode,
                FullAddress = address.FullAddress
            };

            return View(model);
        }

        // POST: Formu kaydet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAddress(AddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var address = await _context.Addresses.FindAsync(model.Id);
                if (address == null)
                    return NotFound();

                address.Title = model.Title;
                address.City = model.City;
                address.District = model.District;
                address.ZipCode = model.ZipCode;
                address.FullAddress = model.FullAddress;

                await _context.SaveChangesAsync();

                TempData["Message"] = "Adres başarıyla güncellendi.";
                return RedirectToAction("Address");
            }

            return View(model);
        }


        public IActionResult Favorite()
        {

            var userId = User.Identity.Name; // giriş yapan kullanıcı
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var favorites = _context.Favorites
                .Where(f => f.UserId == userId)
                .Include(f => f.Book) // kitap bilgilerini de getir
                .ToList();

            return View(favorites);
        }

        public IActionResult PersonalInformation()
        {
            return View();
        }

       
    
            [HttpGet]
            public IActionResult ChangePassword()
            {
                return View();
            }

            // POST: Şifreyi güncelle
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Giriş yapan kullanıcıyı al
                var email = User.Identity?.Name;
                if (string.IsNullOrEmpty(email))
                {
                    return RedirectToAction("Login", "Users");
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    return NotFound();
                }

                // Mevcut şifre doğru mu kontrol et
                if (user.Password != model.CurrentPassword)
                {
                    ModelState.AddModelError(string.Empty, "Mevcut şifre yanlış.");
                    return View(model);
                }

                // Şifreyi güncelle
                user.Password = model.NewPassword;
                await _context.SaveChangesAsync();

                TempData["Message"] = "Şifreniz başarıyla güncellendi.";
                return RedirectToAction("Index");
            }


        }




}
