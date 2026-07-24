using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using _40Let.Data;
using _40Let.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace _40Let.Features;

public class AuthService(AppDbContext context, IOptions<JwtOptions> options) : IAuthService
{
    private readonly JwtOptions _jwt = options.Value;

    public async Task<TokenView?> Login(LoginView view)
    {
        var user = await context.BotUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.PhoneNumber == view.PhoneNumber);

        return user is null ? null : GenerateToken(user);
    }

    public async Task<TokenView?> LoginByClientId(long clientId)
    {
        var user = await context.BotUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == clientId);

        return user is null ? null : GenerateToken(user);
    }

    private TokenView GenerateToken(BotUser user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("chat_id", user.ChatId.ToString()),
            // Short "role" rather than the ClaimTypes.Role URI so the Mini App can
            // read it straight out of the payload. Program.cs matches it via
            // TokenValidationParameters.RoleClaimType.
            new("role", user.Role ?? "user")
        };

        if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
            claims.Add(new Claim("phone_number", user.PhoneNumber));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new TokenView
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt
        };
    }
}
