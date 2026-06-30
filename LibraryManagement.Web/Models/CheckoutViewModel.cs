using System.Collections.Generic;

namespace LibraryManagement.Web.Models
{
    public class CheckoutViewModel
    {
        public int? AddressId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string CardHolderName { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
        public string ExpirationDate { get; set; } = string.Empty;
        public string Cvv { get; set; } = string.Empty;
        public string CouponCode { get; set; } = string.Empty;
        public List<AddressViewModel> Addresses { get; set; } = new();
        public List<CartItemViewModel> CartItems { get; set; } = new();
        public decimal SubTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get; set; }
    }
}
