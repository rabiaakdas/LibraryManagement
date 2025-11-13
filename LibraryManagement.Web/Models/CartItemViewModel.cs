namespace LibraryManagement.Web.Models
{
       public class CartItemViewModel
        {
            public int BookId { get; set; }
            public string Title { get; set; }
            public string ImageUrl { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public decimal TotalPrice => Price * Quantity;
        }
    
}
