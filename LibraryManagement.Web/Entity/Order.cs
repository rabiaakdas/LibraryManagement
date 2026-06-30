using System;
using System.Collections.Generic;

namespace LibraryManagement.Web.Entity
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty; // Kullanıcı kimliği
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";
        public int? AddressId { get; set; }
        public Address Address { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal ShippingFee { get; set; }
        public string CouponCode { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public string CargoCompany { get; set; } = string.Empty;
        public string TrackingNumber { get; set; } = string.Empty;
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
