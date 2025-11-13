using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Web.Entity
{
    public class BookSuggestion
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Author { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [EmailAddress]
        public string? SuggestedByEmail { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int Likes { get; set; } = 0; 
    }
}
