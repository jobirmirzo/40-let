namespace _40Let.Features;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth").WithTags("Auth");

        // POST /auth/token -> issues a signed JWT for a known user.
        group.MapPost("/token", async (LoginView view, IAuthService auth) =>
        {
            var token = await auth.Login(view);
            return token is null
                ? Results.Unauthorized()
                : Results.Ok(token);
        });

        return app;
    }
}
