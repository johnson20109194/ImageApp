using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ImageApp.Application.Interfaces;
using ImageApp.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ImageApp.Infrastructure.Identity;

public class JwtTokenGenerator(IOptions<JwtOptions> opt, IHttpContextAccessor httpContextAccessor)
    : IJwtTokenGenerator
{
    private readonly JwtOptions _opt = opt.Value;

    public string Generate(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.DisplayName),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _opt.Issuer,
            audience: _opt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_opt.ExpiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string? CurrentUsername()
    {
        if (httpContextAccessor.HttpContext?.User.Identity is not ClaimsIdentity { IsAuthenticated: true } identity)
            return string.Empty;
        var username = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        return username;
    }

    public Guid CurrentUserId()
    {
        if (httpContextAccessor.HttpContext?.User.Identity is not ClaimsIdentity { IsAuthenticated: true } identity)
            return Guid.Empty;
        var userId = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        return userId != null ? Guid.Parse(userId) : Guid.Empty;
    }
}