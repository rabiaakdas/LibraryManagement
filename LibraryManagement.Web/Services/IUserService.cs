using System.Threading.Tasks;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Models;

namespace LibraryManagement.Web.Services
{
    /// <summary>
    /// Defines user account operations such as registration, login validation, and password changes.
    /// </summary>
    public interface IUserService
    {
        Task RegisterAsync(RegisterViewModel model);
        Task<User> ValidateLoginAsync(LoginViewModel model);
        Task<User> ValidateApiLoginAsync(string userNameOrEmail, string password);
        Task<User> GetByEmailAsync(string email);
        Task<User> FindOrCreateExternalUserAsync(string email, string name, string provider, string providerUserId);
        Task<int?> GetUserIdByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UsernameExistsAsync(string username);
        Task<(bool Success, string Error)> ChangePasswordAsync(string email, ChangePasswordViewModel model);
    }
}
