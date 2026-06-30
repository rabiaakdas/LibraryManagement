using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LibraryManagement.Web.Entity;

namespace LibraryManagement.Web.Models
{
    public class BookDetailsViewModel
    {
        public Book Book { get; set; }
        public List<BookReview> Reviews { get; set; } = new();
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public bool CanReview { get; set; }
        public bool HasReviewed { get; set; }
        public BookReviewCreateViewModel NewReview { get; set; } = new();
    }

    public class BookReviewCreateViewModel
    {
        public int BookId { get; set; }
        public int Rating { get; set; } = 5;
        public string Comment { get; set; } = string.Empty;
    }

    public class BookReviewSummaryViewModel
    {
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}
