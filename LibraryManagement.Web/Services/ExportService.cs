using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using LibraryManagement.Web.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Web.Services
{
    public class ExportService : IExportService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IBookService _bookService;
        private readonly ILogger<ExportService> _logger;

        public ExportService(
            IOrderRepository orderRepository,
            IBookRepository bookRepository,
            IBookService bookService,
            ILogger<ExportService> logger)
        {
            _orderRepository = orderRepository;
            _bookRepository = bookRepository;
            _bookService = bookService;
            _logger = logger;
        }

        public async Task<byte[]> CreateOrdersReportAsync()
        {
            try
            {
                var orders = await _orderRepository.QueryWithItems()
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Siparisler");

                AddHeader(worksheet, new[]
                {
                    "Siparis Id",
                    "Kullanici",
                    "Siparis Tarihi",
                    "Durum",
                    "Ara Toplam",
                    "Kargo Ucreti",
                    "Indirim Tutari",
                    "Genel Toplam",
                    "Odeme Yontemi",
                    "Kupon Kodu",
                    "Kargo Sirketi",
                    "Takip Numarasi"
                });

                var row = 2;
                foreach (var order in orders)
                {
                    worksheet.Cell(row, 1).Value = order.Id;
                    worksheet.Cell(row, 2).Value = order.UserId;
                    worksheet.Cell(row, 3).Value = order.OrderDate;
                    worksheet.Cell(row, 4).Value = order.Status;
                    worksheet.Cell(row, 5).Value = order.TotalAmount;
                    worksheet.Cell(row, 6).Value = order.ShippingFee;
                    worksheet.Cell(row, 7).Value = order.DiscountAmount;
                    worksheet.Cell(row, 8).Value = order.GrandTotal == 0 ? order.TotalAmount : order.GrandTotal;
                    worksheet.Cell(row, 9).Value = order.PaymentMethod;
                    worksheet.Cell(row, 10).Value = order.CouponCode;
                    worksheet.Cell(row, 11).Value = order.CargoCompany;
                    worksheet.Cell(row, 12).Value = order.TrackingNumber;
                    row++;
                }

                FormatMoneyColumns(worksheet, 5, 8);
                worksheet.Column(3).Style.DateFormat.Format = "dd.MM.yyyy HH:mm";
                worksheet.Columns().AdjustToContents();

                var bytes = SaveWorkbook(workbook);
                _logger.LogInformation("Siparis raporu indirildi. OrderCount: {OrderCount}", orders.Count);
                return bytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rapor olusturma hatasi. ReportType: OrdersExcel");
                throw;
            }
        }

        public async Task<byte[]> CreateStockReportAsync()
        {
            try
            {
                var books = await _bookRepository.QueryWithGenres()
                    .OrderBy(b => b.Title)
                    .ToListAsync();

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Stok");

                AddHeader(worksheet, new[]
                {
                    "Kitap Id",
                    "Kitap Adi",
                    "Yazar",
                    "Fiyat",
                    "Stok",
                    "Stok Durumu",
                    "Kategoriler"
                });

                var row = 2;
                foreach (var book in books)
                {
                    worksheet.Cell(row, 1).Value = book.BookId;
                    worksheet.Cell(row, 2).Value = book.Title;
                    worksheet.Cell(row, 3).Value = book.Author;
                    worksheet.Cell(row, 4).Value = book.Price;
                    worksheet.Cell(row, 5).Value = book.Stock;
                    worksheet.Cell(row, 6).Value = GetStockStatusText(book.Stock);
                    worksheet.Cell(row, 7).Value = book.Genres.Any()
                        ? string.Join(", ", book.Genres.Select(g => g.Name))
                        : "Kategori yok";
                    row++;
                }

                FormatMoneyColumns(worksheet, 4, 4);
                worksheet.Columns().AdjustToContents();

                var bytes = SaveWorkbook(workbook);
                _logger.LogInformation("Stok raporu indirildi. BookCount: {BookCount}", books.Count);
                return bytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rapor olusturma hatasi. ReportType: StockExcel");
                throw;
            }
        }

        private string GetStockStatusText(int stock)
        {
            return _bookService.GetStockStatus(stock) switch
            {
                "OutOfStock" => "Stok Yok",
                "LowStock" => "Dusuk Stok",
                _ => "Stok Var"
            };
        }

        private static void AddHeader(IXLWorksheet worksheet, string[] headers)
        {
            for (var i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = worksheet.Range(1, 1, 1, headers.Length);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        }

        private static void FormatMoneyColumns(IXLWorksheet worksheet, int firstColumn, int lastColumn)
        {
            for (var column = firstColumn; column <= lastColumn; column++)
            {
                worksheet.Column(column).Style.NumberFormat.Format = "#,##0.00 [$₺-tr-TR]";
            }
        }

        private static byte[] SaveWorkbook(XLWorkbook workbook)
        {
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
