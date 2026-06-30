using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Models.Api.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LibraryManagement.Web.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ApiLoginResponseDto CreateToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = jwtSettings["Key"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expireMinutes = int.TryParse(jwtSettings["ExpireMinutes"], out var minutes) ? minutes : 60;

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidOperationException("JWT Key ayarı bulunamadı.");
            }

            var expiration = DateTime.UtcNow.AddMinutes(expireMinutes);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            return new ApiLoginResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration,
                UserName = user.Username,
                Email = user.Email,
                Role = user.Role
            };
        }
    }
}