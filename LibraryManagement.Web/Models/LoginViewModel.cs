using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Web.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Kullanıcı Adı")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }
    }
}
