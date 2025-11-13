using LibraryManagement.Web.Entity;
using System.Collections.Generic;

namespace LibraryManagement.Web.Models
{
    public class BooksViewModel
    {
        public List<Book> Books { get; set; } = new();
        public List<Genre> Genres { get; set; } = new();
    }
}
