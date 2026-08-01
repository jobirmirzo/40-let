using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace _40Let.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Reads the current user's id from the JWT "sub" claim (BotUser.Id).
    /// </summary>
    public static long? GetUserId(this ClaimsPrincipal principal)
    {
        var sub = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return long.TryParse(sub, out var id) ? id : null;
    }
}
