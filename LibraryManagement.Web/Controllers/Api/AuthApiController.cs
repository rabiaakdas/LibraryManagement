using System.Threading.Tasks;
using LibraryManagement.Web.Models.Api.Auth;
using LibraryManagement.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Web.Controllers.Api
{
    [ApiController]
    [Route("api/auth")]
    public class AuthApiController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthApiController(IUserService userService, IJwtTokenService jwtTokenService)
        {
            _userService = userService;
            _jwtTokenService = jwtTokenService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(ApiLoginRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.ValidateApiLoginAsync(model.UserNameOrEmail, model.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Kullanıcı adı/e-posta veya şifre hatalı." });
            }

            return Ok(_jwtTokenService.CreateToken(user));
        }
    }
}