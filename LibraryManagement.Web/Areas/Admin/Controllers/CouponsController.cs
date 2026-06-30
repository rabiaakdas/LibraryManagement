using System.Threading.Tasks;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CouponsController : Controller
    {
        private readonly ICouponService _couponService;

        public CouponsController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _couponService.GetAllAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var coupon = await _couponService.GetByIdAsync(id);
            return coupon == null ? NotFound() : View(coupon);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Coupon { IsActive = true, DiscountType = "Percentage" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon coupon)
        {
            if (!ModelState.IsValid)
            {
                return View(coupon);
            }

            await _couponService.CreateAsync(coupon);
            TempData["Message"] = "Kupon başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var coupon = await _couponService.GetByIdAsync(id);
            return coupon == null ? NotFound() : View(coupon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Coupon coupon)
        {
            if (!ModelState.IsValid)
            {
                return View(coupon);
            }

            var updated = await _couponService.UpdateAsync(id, coupon);
            if (!updated)
            {
                return NotFound();
            }

            TempData["Message"] = "Kupon başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            var updated = await _couponService.DeactivateAsync(id);
            TempData[updated ? "Message" : "Error"] = updated
                ? "Kupon pasifleştirildi."
                : "Kupon bulunamadı.";

            return RedirectToAction(nameof(Index));
        }
    }
}
