using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Web.Models.Api.Auth
{
    public class ApiLoginRequestDto
    {
        public string UserNameOrEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}