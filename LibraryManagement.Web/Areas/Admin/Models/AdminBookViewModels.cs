using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LibraryManagement.Web.Entity;

namespace LibraryManagement.Web.Areas.Admin.Models
{
    public class AdminBookFormViewModel
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;
        public int PageCount { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public List<int> SelectedGenreIds { get; set; } = new();
        public List<AdminGenreCheckboxViewModel> Genres { get; set; } = new();
    }

    public class AdminGenreCheckboxViewModel
    {
        public int GenreId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }

    public class AdminBookListViewModel
    {
        public string StockFilter { get; set; } = string.Empty;
        public List<Book> Books { get; set; } = new();
    }
}
