using System;
using System.Threading.Tasks;
using LibraryManagement.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly IExportService _exportService;

        public ReportsController(IExportService exportService)
        {
            _exportService = exportService;
        }

        [HttpGet("Admin/Reports/OrdersExcel")]
        public async Task<IActionResult> OrdersExcel()
        {
            var bytes = await _exportService.CreateOrdersReportAsync();
            var fileName = $"orders-report-{DateTime.Now:yyyyMMdd}.xlsx";
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet("Admin/Reports/StockExcel")]
        public async Task<IActionResult> StockExcel()
        {
            var bytes = await _exportService.CreateStockReportAsync();
            var fileName = $"stock-report-{DateTime.Now:yyyyMMdd}.xlsx";
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
