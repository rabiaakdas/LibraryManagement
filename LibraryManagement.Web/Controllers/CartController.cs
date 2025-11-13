using LibraryManagement.Web.Data;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Models; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CartController : Controller
{
    private readonly BookContext _context;

    public CartController(BookContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        //Sepeti session’dan veya db’den alıyoruz
        var cart = HttpContext.Session.GetObject<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();
        return View(cart);
    }

    [HttpPost]
    public IActionResult UpdateQuantity(int bookId, int quantity)
    {
        // Session’dan mevcut sepeti al veya yeni liste oluştur
        var cart = HttpContext.Session.GetObject<List<CartItemViewModel>>("Cart") ?? new List<CartItemViewModel>();

        // Sepette kitap var mı kontrol et
        var item = cart.FirstOrDefault(c => c.BookId == bookId);
        if (item != null)
        {
            // Var olan ürünün adedini artır veya yeni adet ata
            item.Quantity = quantity;
        }
        else
        {
            // Yeni kitap ekle
            var book = _context.Books.Find(bookId); // EF Core ile kitap bul
            if (book != null)
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
        }

        // Sepeti session’a kaydet
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

    [Authorize] // Giriş yapılmamışsa kullanıcıyı login sayfasına yönlendirir
    [HttpPost]
    public IActionResult Checkout(List<CartItemViewModel> cart)
    {
        if (cart == null || !cart.Any())
        {
            TempData["Error"] = "Sepetiniz boş!";
            return RedirectToAction("Index", "Cart");
        }

        var userId = User.Identity.Name; // guest yok

        //Sipariş nesnesi oluştur
        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.Now,
            TotalAmount = cart.Sum(c => c.Price * c.Quantity),
            Items = new List<OrderItem>()
        };

        foreach (var item in cart)
        {
            var book = _context.Books.FirstOrDefault(b => b.BookId == item.BookId);
            if (book == null)
                continue;

            // Stok kontrolü
            if (book.Stock < item.Quantity)
            {
                TempData["Error"] = $"{book.Title} için stok yetersiz! Mevcut stok: {book.Stock}";
                return RedirectToAction("Index", "Cart");
            }

            // Stok azalt
            book.Stock -= item.Quantity;

            //Sipariş kalemi ekle
            order.Items.Add(new OrderItem
            {
                BookId = book.BookId,
                Title = book.Title,
                Price = book.Price,
                Quantity = item.Quantity,
                TotalPrice = item.Price * item.Quantity
            });
        }

        _context.Orders.Add(order);
        _context.SaveChanges();

        // Sepeti temizle
        HttpContext.Session.Remove("Cart");

        TempData["Success"] = "Siparişiniz başarıyla alındı!";
        return View("Index", new List<CartItemViewModel>());
    }

    public IActionResult MyOrders()
    {
        var userId = User.Identity?.Name ?? "guest";

        var orders = _context.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new OrderViewModel
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Items = o.Items.Select(i => new OrderItemViewModel
                {
                    Title = i.Title,
                    Quantity = i.Quantity,
                    Price = i.Price,
                    TotalPrice = i.TotalPrice
                }).ToList()
            }).ToList();

        return View(orders);
    }
}
