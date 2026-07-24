namespace _40Let.Features;

public interface IAuthService
{
    /// <summary>Validates the login and, on success, returns a signed JWT.</summary>
    Task<TokenView?> Login(LoginView view);

    /// <summary>Issues a signed JWT for a known client id, or null when no such user exists.</summary>
    Task<TokenView?> LoginByClientId(long clientId);
}
