namespace LibraryManagement.Web.Models
{
    public class OrderItemViewModel
    {
        public string Title { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
