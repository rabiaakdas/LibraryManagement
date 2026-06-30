using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LibraryManagement.Web.Entity;

namespace LibraryManagement.Web.Areas.Admin.Models
{
    public class AdminOrderListItemViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public int ItemCount { get; set; }
    }

    public class AdminOrderListViewModel
    {
        public List<AdminOrderListItemViewModel> Orders { get; set; } = new();
    }

    public class AdminOrderStatusViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string CargoCompany { get; set; } = string.Empty;
        public string TrackingNumber { get; set; } = string.Empty;

        public List<string> StatusOptions { get; set; } = new();
    }

    public class AdminOrderDetailsViewModel
    {
        public Order Order { get; set; }
    }
}
