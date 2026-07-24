using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using _40Let.Models;

namespace _40Let.Features;

public static class BotUserEndpoints
{
    public static IEndpointRouteBuilder MapBotUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users").WithTags("Users");

        group.MapGet("/me", async (ClaimsPrincipal principal, IBotUserService users) =>
        {
            var sub = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (!long.TryParse(sub, out var id))
                return Results.Unauthorized();

            var user = await users.GetById(id);
            return user is null ? Results.NotFound() : Results.Ok(user);
        })
        .RequireAuthorization()
        .WithName("GetCurrentUser")
        .WithSummary("Get the authenticated user")
        .Produces<BotUser>()
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", async (IBotUserService users) =>
            Results.Ok(await users.GetAll()))
            .WithName("GetUsers")
            .WithSummary("List all users")
            .Produces<List<BotUser>>();

        group.MapGet("/{id:long}", async (long id, IBotUserService users) =>
            await users.GetById(id) is { } user ? Results.Ok(user) : Results.NotFound())
            .WithName("GetUserById")
            .WithSummary("Get a user by id")
            .Produces<BotUser>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/by-chat/{chatId:long}", async (long chatId, IBotUserService users) =>
            await users.GetByChatId(chatId) is { } user ? Results.Ok(user) : Results.NotFound())
            .WithName("GetUserByChatId")
            .WithSummary("Get a user by Telegram chat id")
            .Produces<BotUser>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async (BotUserView view, IBotUserService users) =>
        {
            var user = await users.Create(view);
            return Results.Created($"/users/{user.Id}", user);
        })
            .WithName("CreateUser")
            .WithSummary("Create a user")
            .Produces<BotUser>(StatusCodes.Status201Created);

        group.MapPut("/{id:long}", async (long id, BotUserView view, IBotUserService users) =>
            await users.Update(id, view) ? Results.NoContent() : Results.NotFound())
            .WithName("UpdateUser")
            .WithSummary("Update a user")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:long}", async (long id, IBotUserService users) =>
            await users.Delete(id) ? Results.NoContent() : Results.NotFound())
            .WithName("DeleteUser")
            .WithSummary("Delete a user")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}
