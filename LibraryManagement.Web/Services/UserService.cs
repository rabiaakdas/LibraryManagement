using System.Threading.Tasks;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Models;
using LibraryManagement.Web.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace LibraryManagement.Web.Services
{
    /// <summary>
    /// Provides user account business logic for registration, login validation, and password changes.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _users;

        public UserService(IUserRepository users)
        {
            _users = users;
        }

        public async Task RegisterAsync(RegisterViewModel model)
        {
            _users.Add(new User
            {
                Username = model.Username,
                Email = model.Email,
                Password = model.Password,
                Role = "User"
            });

            await _users.SaveChangesAsync();
        }

        public async Task<User> ValidateLoginAsync(LoginViewModel model)
        {
            var user = await _users.GetByUsernameAsync(model.Username);
            if (user != null && IsPasswordValid(user.Password, model.Password))
            {
                return user;
            }

            return null;
        }


        public async Task<User> ValidateApiLoginAsync(string userNameOrEmail, string password)
        {
            if (string.IsNullOrWhiteSpace(userNameOrEmail) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            var normalizedLogin = userNameOrEmail.Trim().ToLower();
            var user = await _users.Query()
                .FirstOrDefaultAsync(u =>
                    u.Username.ToLower() == normalizedLogin ||
                    u.Email.ToLower() == normalizedLogin);

            if (user != null && IsPasswordValid(user.Password, password))
            {
                return user;
            }

            return null;
        }
        public async Task<User> GetByEmailAsync(string email)
        {
            return await _users.GetByEmailAsync(email);
        }

        public async Task<User> FindOrCreateExternalUserAsync(string email, string name, string provider, string providerUserId)
        {
            var existingUser = await _users.GetByEmailAsync(email);
            if (existingUser != null)
            {
                if (string.IsNullOrWhiteSpace(existingUser.Provider))
                {
                    existingUser.Provider = provider;
                    existingUser.ProviderUserId = providerUserId;
                    await _users.SaveChangesAsync();
                }

                return existingUser;
            }

            var username = await GenerateUniqueUsernameAsync(email, name);
            var user = new User
            {
                Username = username,
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString("N")),
                Role = "User",
                Provider = provider,
                ProviderUserId = providerUserId
            };

            _users.Add(user);
            await _users.SaveChangesAsync();
            return user;
        }

        public async Task<int?> GetUserIdByEmailAsync(string email)
        {
            return await _users.GetUserIdByEmailAsync(email);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _users.Query().AnyAsync(u => u.Email == email);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _users.Query().AnyAsync(u => u.Username == username);
        }

        public async Task<(bool Success, string Error)> ChangePasswordAsync(string email, ChangePasswordViewModel model)
        {
            var user = await _users.GetByEmailAsync(email);
            if (user == null)
            {
                return (false, "Kullanıcı bulunamadı.");
            }

            if (user.Password != model.CurrentPassword)
            {
                return (false, "Mevcut şifre yanlış.");
            }

            user.Password = model.NewPassword;
            await _users.SaveChangesAsync();
            return (true, null);
        }

        private static bool IsPasswordValid(string storedPassword, string password)
        {
            if (storedPassword == password)
            {
                return true;
            }

            if (storedPassword.StartsWith("$2"))
            {
                return BCrypt.Net.BCrypt.Verify(password, storedPassword);
            }

            return false;
        }

        private async Task<string> GenerateUniqueUsernameAsync(string email, string name)
        {
            var source = !string.IsNullOrWhiteSpace(name) ? name : email.Split('@').FirstOrDefault();
            var baseUsername = Regex.Replace(source ?? "user", "[^a-zA-Z0-9]", string.Empty).ToLower();
            if (string.IsNullOrWhiteSpace(baseUsername))
            {
                baseUsername = "user";
            }

            var username = baseUsername;
            var counter = 1;

            while (await UsernameExistsAsync(username))
            {
                username = $"{baseUsername}{counter}";
                counter++;
            }

            return username;
        }
    }
}
