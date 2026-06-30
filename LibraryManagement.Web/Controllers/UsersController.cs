using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Models;
using LibraryManagement.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, IConfiguration configuration, ILogger<UsersController> logger)
        {
            _userService = userService;
            _configuration = configuration;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _userService.RegisterAsync(model);
                return RedirectToAction("Login");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            SetGoogleLoginViewData();
            return View();
        }

        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.ValidateLoginAsync(model);

                if (user != null)
                {
                    await SignInUserAsync(user);

                    _logger.LogInformation(
                        "Kullanici login basarili. UserId: {UserId}, Username: {Username}, Email: {Email}",
                        user.UserId,
                        user.Username,
                        user.Email);

                    return RedirectToAction("Index", "Profiles");
                }

                _logger.LogInformation(
                    "Kullanici login basarisiz. Username: {Username}, Email: {Email}",
                    model.Username,
                    model.Email);

                ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
            }
            else
            {
                _logger.LogInformation(
                    "Kullanici login basarisiz. Model validation hatasi. Username: {Username}, Email: {Email}",
                    model.Username,
                    model.Email);
            }

            SetGoogleLoginViewData();
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            if (!IsGoogleLoginConfigured())
            {
                _logger.LogWarning("Google login basarisiz. Google ayarlari yapilandirilmamis.");
                TempData["Error"] = "Google giriş ayarları yapılandırılmamış.";
                return RedirectToAction(nameof(Login));
            }

            if (provider != GoogleDefaults.AuthenticationScheme)
            {
                _logger.LogWarning("Google login basarisiz. Gecersiz provider: {Provider}", provider);
                return RedirectToAction(nameof(Login));
            }

            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Users", new { returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, provider);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            if (!IsGoogleLoginConfigured())
            {
                _logger.LogWarning("Google login basarisiz. Callback cagrildi ancak Google ayarlari yapilandirilmamis.");
                TempData["Error"] = "Google giriş ayarları yapılandırılmamış.";
                return RedirectToAction(nameof(Login));
            }

            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded || authenticateResult.Principal == null)
            {
                _logger.LogWarning("Google login basarisiz. External authenticate sonucu basarisiz.");
                TempData["Error"] = "Google ile giriş yapılamadı.";
                return RedirectToAction(nameof(Login));
            }

            var email = authenticateResult.Principal.FindFirstValue(ClaimTypes.Email);
            var name = authenticateResult.Principal.FindFirstValue(ClaimTypes.Name) ?? email;
            var providerUserId = authenticateResult.Principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(providerUserId))
            {
                _logger.LogWarning("Google login basarisiz. Email veya provider user id bos.");
                TempData["Error"] = "Google hesabından gerekli kullanıcı bilgileri alınamadı.";
                return RedirectToAction(nameof(Login));
            }

            var existingUser = await _userService.GetByEmailAsync(email);
            var user = await _userService.FindOrCreateExternalUserAsync(email, name, GoogleDefaults.AuthenticationScheme, providerUserId);

            if (existingUser == null)
            {
                _logger.LogInformation(
                    "Google ile yeni kullanici olusturuldu. UserId: {UserId}, Email: {Email}",
                    user.UserId,
                    user.Email);
            }

            await SignInUserAsync(user);

            _logger.LogInformation(
                "Google login basarili. UserId: {UserId}, Username: {Username}, Email: {Email}",
                user.UserId,
                user.Username,
                user.Email);

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction("Index", "Profiles");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Users");
        }

        private async Task SignInUserAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim("Username", user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        private void SetGoogleLoginViewData()
        {
            ViewBag.IsGoogleLoginConfigured = IsGoogleLoginConfigured();
        }

        private bool IsGoogleLoginConfigured()
        {
            var clientId = _configuration["Authentication:Google:ClientId"];
            var clientSecret = _configuration["Authentication:Google:ClientSecret"];

            return !string.IsNullOrWhiteSpace(clientId)
                && !string.IsNullOrWhiteSpace(clientSecret)
                && clientId != "YOUR_GOOGLE_CLIENT_ID"
                && clientSecret != "YOUR_GOOGLE_CLIENT_SECRET";
        }
    }
}
