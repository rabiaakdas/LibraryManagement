using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Web.Models.Api
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public double RatingAverage { get; set; }
        public int ReviewCount { get; set; }
        public List<string> Genres { get; set; } = new();
    }

    public class BookUpsertDto
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;
        public int PageCount { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public List<int> GenreIds { get; set; } = new();
    }
}
