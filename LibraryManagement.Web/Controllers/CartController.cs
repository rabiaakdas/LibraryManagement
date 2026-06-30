using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Web.Models;
using LibraryManagement.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class CartController : Controller
{
    private readonly IBookService _bookService;
    private readonly IOrderService _orderService;
    private readonly IAddressService _addressService;
    private readonly ICouponService _couponService;
    private readonly IInvoiceService _invoiceService;

    public CartController(
        IBookService bookService,
        IOrderService orderService,
        IAddressService addressService,
        ICouponService couponService,
        IInvoiceService invoiceService)
    {
        _bookService = bookService;
        _orderService = orderService;
        _addressService = addressService;
        _couponService = couponService;
        _invoiceService = invoiceService;
    }

    public IActionResult Index()
    {
        var cart = HttpContext.Session.GetObject<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();
        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateQuantity(int bookId, int quantity)
    {
        if (quantity < 1)
        {
            TempData["Error"] = "Adet en az 1 olmalıdır.";
            return RedirectToAction(nameof(Index));
        }

        var cart = HttpContext.Session.GetObject<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();
        var item = cart.FirstOrDefault(c => c.BookId == bookId);
        var book = await _bookService.GetBookWithGenresAsync(bookId);

        if (book == null)
        {
            TempData["Error"] = "Kitap bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        if (book.Stock <= 0)
        {
            TempData["Error"] = $"{book.Title} şu anda stokta yok.";
            return RedirectToAction(nameof(Index));
        }

        if (quantity > book.Stock)
        {
            TempData["Error"] = $"{book.Title} için en fazla {book.Stock} adet ekleyebilirsiniz.";
            return RedirectToAction(nameof(Index));
        }

        if (item != null)
        {
            item.Quantity = quantity;
        }
        else
        {
            cart.Add(new CartItemViewModel
            {
                BookId = book.BookId,
                Title = book.Title,
                ImageUrl = book.ImageUrl,
                Price = book.Price,
                Quantity = quantity
            });
        }

        HttpContext.Session.SetObject("Cart", cart);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Remove(int bookId)
    {
        var cart = HttpContext.Session.GetObject<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();
        cart.RemoveAll(c => c.BookId == bookId);
        HttpContext.Session.SetObject("Cart", cart);
        return RedirectToAction("Index");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var cart = HttpContext.Session.GetObject<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();
        if (!cart.Any())
        {
            TempData["Error"] = "Sepetiniz boş!";
            return RedirectToAction(nameof(Index));
        }

        return View(await CreateCheckoutModelAsync(new CheckoutViewModel()));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApplyCoupon(CheckoutViewModel model)
    {
        var checkoutModel = await CreateCheckoutModelAsync(model);
        if (string.IsNullOrWhiteSpace(model.CouponCode))
        {
            ModelState.AddModelError(nameof(model.CouponCode), "Kupon kodu girmelisiniz.");
            return View("Checkout", checkoutModel);
        }

        var couponResult = await _couponService.ValidateCouponAsync(model.CouponCode, checkoutModel.SubTotal);
        if (!couponResult.Success)
        {
            ModelState.AddModelError(nameof(model.CouponCode), couponResult.Message);
            checkoutModel.CouponCode = model.CouponCode;
            return View("Checkout", checkoutModel);
        }

        checkoutModel.CouponCode = couponResult.CouponCode;
        checkoutModel.DiscountAmount = couponResult.DiscountAmount;
        checkoutModel.GrandTotal = checkoutModel.SubTotal + checkoutModel.ShippingFee - checkoutModel.DiscountAmount;
        TempData["Success"] = couponResult.Message;
        return View("Checkout", checkoutModel);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel model)
    {
        var cart = HttpContext.Session.GetObject<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();
        model.CartItems = cart;

        if (!ModelState.IsValid)
        {
            return View(await CreateCheckoutModelAsync(model));
        }

        var result = await _orderService.CheckoutAsync(model, User.Identity.Name);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error);
            return View(await CreateCheckoutModelAsync(model));
        }

        HttpContext.Session.Remove("Cart");
        TempData["Success"] = "Siparişiniz başarıyla alındı!";
        return RedirectToAction(nameof(MyOrders));
    }

    public async Task<IActionResult> MyOrders()
    {
        var userId = User.Identity?.Name ?? "guest";
        return View(await _orderService.GetUserOrdersAsync(userId));
    }

    [Authorize]
    [HttpGet("Cart/Invoice/{orderId:int}")]
    public async Task<IActionResult> Invoice(int orderId)
    {
        var result = await _invoiceService.CreateInvoiceAsync(orderId, User.Identity.Name);
        if (result.NotFound)
        {
            return NotFound();
        }

        if (result.Forbidden)
        {
            return Forbid();
        }

        if (!result.Success)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction(nameof(MyOrders));
        }

        return File(result.PdfBytes, "application/pdf", result.FileName);
    }

    private async Task<CheckoutViewModel> CreateCheckoutModelAsync(CheckoutViewModel model)
    {
        var cart = HttpContext.Session.GetObject<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();
        model.CartItems = cart;
        model.Addresses = await _addressService.GetUserAddressesAsync(User.Identity.Name);
        model.SubTotal = cart.Sum(x => x.TotalPrice);
        model.ShippingFee = OrderService.CalculateShippingFee(model.SubTotal);
        model.GrandTotal = model.SubTotal + model.ShippingFee - model.DiscountAmount;
        return model;
    }
}
