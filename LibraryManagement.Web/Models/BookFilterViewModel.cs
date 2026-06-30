using System.Collections.Generic;
using LibraryManagement.Web.Entity;

namespace LibraryManagement.Web.Models
{
    public class BookFilterViewModel
    {
        public string Search { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool InStockOnly { get; set; }
        public string Sort { get; set; } = "name_asc";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 9;
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public List<Book> Books { get; set; } = new();
        public List<Genre> Categories { get; set; } = new();
        public Dictionary<int, BookReviewSummaryViewModel> ReviewSummaries { get; set; } = new();
    }
}
