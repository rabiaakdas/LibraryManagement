using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Web.Models
{
    public class RegisterViewModel
    {
        [Display(Name = "Kullanıcı Adı")]
        public string Username { get; set; } = string.Empty;
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; } = string.Empty;
    }
}
