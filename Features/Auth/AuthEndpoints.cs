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
        })
        .WithName("Login")
        .WithSummary("Issue a JWT for a known user")
        .Produces<TokenView>()
        .Produces(StatusCodes.Status401Unauthorized);

        // POST /auth/token/{clientId} -> issues a signed JWT for a client id.
        // The bot puts that id in the Mini App URL (?clientId=...), and the app
        // trades it for a token instead of trusting a role from the query string.
        group.MapPost("/token/{clientId:long}", async (long clientId, IAuthService auth) =>
        {
            var token = await auth.LoginByClientId(clientId);
            return token is null
                ? Results.Unauthorized()
                : Results.Ok(token);
        })
        .WithName("LoginByClientId")
        .WithSummary("Issue a JWT for a client id")
        .Produces<TokenView>()
        .Produces(StatusCodes.Status401Unauthorized);

        return app;
    }
}
