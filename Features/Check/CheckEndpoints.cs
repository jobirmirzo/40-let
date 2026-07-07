namespace _40Let.Features;

public static class CheckEndpoints
{
    public static IEndpointRouteBuilder MapCheckEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/checks").WithTags("Checks");

        group.MapGet("/", async (ICheckService checks) =>
            Results.Ok(await checks.GetAll()));

        group.MapGet("/{id:long}", async (long id, ICheckService checks) =>
            await checks.GetById(id) is { } check ? Results.Ok(check) : Results.NotFound());

        group.MapPost("/", async (CheckView view, ICheckService checks) =>
        {
            var check = await checks.Create(view);
            return Results.Created($"/checks/{check.Id}", check);
        });

        group.MapPut("/{id:long}", async (long id, CheckView view, ICheckService checks) =>
            await checks.Update(id, view) ? Results.NoContent() : Results.NotFound());

        group.MapDelete("/{id:long}", async (long id, ICheckService checks) =>
            await checks.Delete(id) ? Results.NoContent() : Results.NotFound());

        return app;
    }
}
