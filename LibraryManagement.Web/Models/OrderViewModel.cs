using System;
using System.Collections.Generic;

namespace LibraryManagement.Web.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemViewModel> Items { get; set; } = new();
    }
}
