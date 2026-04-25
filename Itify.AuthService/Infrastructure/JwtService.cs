using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Itify.AuthService.DataTransferObjects;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Itify.AuthService.Infrastructure;

public class JwtService(IOptions<JwtConfiguration> options)
{
    private readonly JwtConfiguration _config = options.Value;

    public string GenerateToken(DbUserRecord user)
    {
        var key = Encoding.ASCII.GetBytes(_config.Key);
        var now = DateTime.UtcNow;

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())]),
            Claims = new Dictionary<string, object>
            {
                { ClaimTypes.Name, user.Name },
                { ClaimTypes.Email, user.Email },
                { ClaimTypes.Role, user.Role.ToString() }
            },
            IssuedAt = now,
            Expires = now.AddDays(7),
            Issuer = _config.Issuer,
            Audience = _config.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(handler.CreateToken(descriptor));
    }
}
