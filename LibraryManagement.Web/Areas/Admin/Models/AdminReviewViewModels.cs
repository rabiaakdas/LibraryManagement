using System;
using System.Collections.Generic;
using LibraryManagement.Web.Entity;

namespace LibraryManagement.Web.Areas.Admin.Models
{
    public class AdminReviewFilterViewModel
    {
        public string BookSearch { get; set; } = string.Empty;
        public string UserSearch { get; set; } = string.Empty;
        public int? Rating { get; set; }
        public string Sort { get; set; } = "newest";
        public List<BookReview> Reviews { get; set; } = new();
    }
}
