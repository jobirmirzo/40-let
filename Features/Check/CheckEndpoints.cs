using _40Let.Models;

namespace _40Let.Features;

public static class CheckEndpoints
{
    public static IEndpointRouteBuilder MapCheckEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/checks").WithTags("Checks");

        group.MapGet("/", async (ICheckService checks) =>
            Results.Ok(await checks.GetAll()))
            .WithName("GetChecks")
            .WithSummary("List all checks")
            .Produces<List<Check>>();

        group.MapGet("/{id:long}", async (long id, ICheckService checks) =>
            await checks.GetById(id) is { } check ? Results.Ok(check) : Results.NotFound())
            .WithName("GetCheckById")
            .WithSummary("Get a check by id")
            .Produces<Check>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async (CheckView view, ICheckService checks) =>
        {
            var check = await checks.Create(view);
            return Results.Created($"/checks/{check.Id}", check);
        })
            .WithName("CreateCheck")
            .WithSummary("Create a check")
            .Produces<Check>(StatusCodes.Status201Created);

        group.MapPut("/{id:long}", async (long id, CheckView view, ICheckService checks) =>
            await checks.Update(id, view) ? Results.NoContent() : Results.NotFound())
            .WithName("UpdateCheck")
            .WithSummary("Update a check")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:long}", async (long id, ICheckService checks) =>
            await checks.Delete(id) ? Results.NoContent() : Results.NotFound())
            .WithName("DeleteCheck")
            .WithSummary("Delete a check")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}
