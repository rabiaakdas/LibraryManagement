using System;
using System.Collections.Generic;

namespace LibraryManagement.Web.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal ShippingFee { get; set; }
        public string CouponCode { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public string AddressTitle { get; set; } = string.Empty;
        public string AddressDetail { get; set; } = string.Empty;
        public string CargoCompany { get; set; } = string.Empty;
        public string TrackingNumber { get; set; } = string.Empty;
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<OrderItemViewModel> Items { get; set; } = new();
    }
}
