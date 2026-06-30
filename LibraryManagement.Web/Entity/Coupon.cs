using System;

namespace LibraryManagement.Web.Entity
{
    public class Coupon
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string DiscountType { get; set; } = "Percentage";
        public decimal DiscountValue { get; set; }
        public decimal MinimumOrderAmount { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? ExpirationDate { get; set; }
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
