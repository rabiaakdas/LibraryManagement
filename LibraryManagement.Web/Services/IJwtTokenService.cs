using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Models.Api.Auth;

namespace LibraryManagement.Web.Services
{
    public interface IJwtTokenService
    {
        ApiLoginResponseDto CreateToken(User user);
    }
}