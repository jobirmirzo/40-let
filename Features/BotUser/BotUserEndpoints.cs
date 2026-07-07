using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace _40Let.Features;

public static class BotUserEndpoints
{
    public static IEndpointRouteBuilder MapBotUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users").WithTags("Users");

        // GET /users/me -> the user identified by the bearer token.
        group.MapGet("/me", async (ClaimsPrincipal principal, IBotUserService users) =>
        {
            var sub = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (!long.TryParse(sub, out var id))
                return Results.Unauthorized();

            var user = await users.GetById(id);
            return user is null ? Results.NotFound() : Results.Ok(user);
        })
        .RequireAuthorization();

        group.MapGet("/", async (IBotUserService users) =>
            Results.Ok(await users.GetAll()));

        group.MapGet("/{id:long}", async (long id, IBotUserService users) =>
            await users.GetById(id) is { } user ? Results.Ok(user) : Results.NotFound());

        group.MapPost("/", async (BotUserView view, IBotUserService users) =>
        {
            var user = await users.Create(view);
            return Results.Created($"/users/{user.Id}", user);
        });

        group.MapPut("/{id:long}", async (long id, BotUserView view, IBotUserService users) =>
            await users.Update(id, view) ? Results.NoContent() : Results.NotFound());

        group.MapDelete("/{id:long}", async (long id, IBotUserService users) =>
            await users.Delete(id) ? Results.NoContent() : Results.NotFound());

        return app;
    }
}
