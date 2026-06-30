using System;
using System.Collections.Generic;

namespace LibraryManagement.Web.Areas.Admin.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalBooks { get; set; }
        public int TotalUsers { get; set; }
        public int TotalOrders { get; set; }
        public int TotalGenres { get; set; }
        public int TotalReviews { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingOrders { get; set; }
        public int OutOfStockBooks { get; set; }

        public List<DashboardOrderViewModel> LatestOrders { get; set; } = new();
        public List<DashboardBestSellerViewModel> BestSellingBooks { get; set; } = new();
        public List<DashboardReviewViewModel> LatestReviews { get; set; } = new();
        public List<DashboardLowStockBookViewModel> LowStockBooks { get; set; } = new();
    }

    public class DashboardOrderViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class DashboardBestSellerViewModel
    {
        public string Title { get; set; } = string.Empty;
        public int TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class DashboardReviewViewModel
    {
        public int Id { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class DashboardLowStockBookViewModel
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public string StockStatus { get; set; } = string.Empty;
    }
}
