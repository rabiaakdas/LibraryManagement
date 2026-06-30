namespace LibraryManagement.Web.Models
{
    public class CouponResultViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string CouponCode { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
    }
}
