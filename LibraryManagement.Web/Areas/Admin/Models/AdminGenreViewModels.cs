using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Web.Areas.Admin.Models
{
    public class AdminGenreFormViewModel
    {
        public int GenreId { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class AdminGenreListItemViewModel
    {
        public int GenreId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int BookCount { get; set; }
    }

    public class AdminGenreListViewModel
    {
        public List<AdminGenreListItemViewModel> Genres { get; set; } = new();
    }
}
