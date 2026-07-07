namespace _40Let.Features;

public interface IAuthService
{
    /// <summary>Validates the login and, on success, returns a signed JWT.</summary>
    Task<TokenView?> Login(LoginView view);
}
