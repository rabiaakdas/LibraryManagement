using LibraryManagement.Web.Entity;
using System.Collections.Generic;

namespace LibraryManagement.Web.Models
{
    public class HomePageViewModel
    {
        public List<Book> PopularBooks { get; set; } = new();
    }
}
