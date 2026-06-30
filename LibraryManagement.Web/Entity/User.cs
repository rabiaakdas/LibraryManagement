using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Web.Entity
{
    public class User
    {
        public int UserId { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Role { get; set; } = "User";

        [MaxLength(50)]
        public string Provider { get; set; } = string.Empty;

        [MaxLength(200)]
        public string ProviderUserId { get; set; } = string.Empty;

        public ICollection<BookReview> Reviews { get; set; } = new List<BookReview>();

    }
}
