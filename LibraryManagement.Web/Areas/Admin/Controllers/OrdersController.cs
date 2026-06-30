using System.Threading.Tasks;
using LibraryManagement.Web.Areas.Admin.Models;
using LibraryManagement.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IInvoiceService _invoiceService;

        public OrdersController(IOrderService orderService, IInvoiceService invoiceService)
        {
            _orderService = orderService;
            _invoiceService = invoiceService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _orderService.GetAdminOrderListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var model = await _orderService.GetAdminOrderDetailsAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditStatus(int id)
        {
            var model = await _orderService.GetEditStatusModelAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStatus(int id, AdminOrderStatusViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var result = await _orderService.UpdateStatusAsync(id, model);
            if (!result.Success)
            {
                ModelState.AddModelError(nameof(model.Status), result.Error);
                var filledModel = await _orderService.GetEditStatusModelAsync(id);
                if (filledModel == null)
                {
                    return NotFound();
                }
                filledModel.Status = model.Status;
                return View(filledModel);
            }

            TempData["Success"] = "Siparis durumu basariyla guncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Admin/Orders/Invoice/{orderId:int}")]
        public async Task<IActionResult> Invoice(int orderId)
        {
            var result = await _invoiceService.CreateInvoiceAsync(orderId, User.Identity.Name, isAdmin: true);
            if (result.NotFound)
            {
                return NotFound();
            }

            if (!result.Success)
            {
                TempData["Error"] = result.Error;
                return RedirectToAction(nameof(Details), new { id = orderId });
            }

            return File(result.PdfBytes, "application/pdf", result.FileName);
        }
    }
}
