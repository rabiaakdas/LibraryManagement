using System.ComponentModel.DataAnnotations;

#nullable enable

namespace LibraryManagement.Web.Models
{
    public class LoginViewModel
    {
        [Display(Name = "Kullanıcı Adı")]
        public string Username { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; } = string.Empty;
        public string? Email { get; set; }
    }
}
