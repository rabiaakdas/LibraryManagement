using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Web.Entity
{
    public class BookReview
    {
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        public Book Book { get; set; }

        [Required]
        public int UserId { get; set; }

        public User User { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [StringLength(500)]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
