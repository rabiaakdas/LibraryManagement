using System;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Web.Models;
using LibraryManagement.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Web.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAddressService _addressService;
        private readonly IFavoriteService _favoriteService;

        public ProfilesController(
            IUserService userService,
            IAddressService addressService,
            IFavoriteService favoriteService)
        {
            _userService = userService;
            _addressService = addressService;
            _favoriteService = favoriteService;
        }

        public async Task<IActionResult> Index()
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Users");
            }

            var user = await _userService.GetByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }

            return View(new ProfileViewModel
            {
                FullName = user.Username,
                Email = user.Email
            });
        }

        public async Task<IActionResult> Address()
        {
            var userId = User.Identity.Name;
            return View(await _addressService.GetUserAddressesAsync(userId));
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

            await _addressService.CreateAddressAsync(model, User.Identity.Name);
            TempData["Message"] = "Adres başarıyla kaydedildi.";
            return RedirectToAction("Address");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var deleted = await _addressService.DeleteAddressAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            TempData["Message"] = "Adres başarıyla silindi.";
            return RedirectToAction("Address");
        }

        [HttpGet]
        public async Task<IActionResult> EditAddress(int id)
        {
            var model = await _addressService.GetAddressModelAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAddress(AddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var updated = await _addressService.UpdateAddressAsync(model);
                if (!updated)
                {
                    return NotFound();
                }

                TempData["Message"] = "Adres başarıyla güncellendi.";
                return RedirectToAction("Address");
            }

            return View(model);
        }

        public async Task<IActionResult> Favorite()
        {
            var userId = User.Identity.Name;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(await _favoriteService.GetUserFavoritesAsync(userId));
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Users");
            }

            var result = await _userService.ChangePasswordAsync(email, model);
            if (!result.Success)
            {
                if (result.Error == "Kullanıcı bulunamadı.")
                {
                    return NotFound();
                }

                ModelState.AddModelError(string.Empty, result.Error);
                return View(model);
            }

            TempData["Message"] = "Şifreniz başarıyla güncellendi.";
            return RedirectToAction("Index");
        }
    }
}
