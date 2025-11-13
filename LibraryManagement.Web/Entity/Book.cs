using System.Collections.Generic;

namespace LibraryManagement.Web.Entity
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ImageUrl { get; set; }
        public int PageCount { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public HashSet<Genre> Genres { get; set; } = new();


    }
}
