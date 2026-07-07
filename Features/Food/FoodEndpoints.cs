namespace _40Let.Features;

public static class FoodEndpoints
{
    public static IEndpointRouteBuilder MapFoodEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/foods").WithTags("Foods");

        group.MapGet("/", async (IFoodService foods) =>
            Results.Ok(await foods.GetAll()));

        group.MapGet("/{id:long}", async (long id, IFoodService foods) =>
            await foods.GetById(id) is { } food ? Results.Ok(food) : Results.NotFound());

        group.MapPost("/", async (FoodView view, IFoodService foods) =>
        {
            var food = await foods.Create(view);
            return Results.Created($"/foods/{food.Id}", food);
        });

        group.MapPut("/{id:long}", async (long id, FoodView view, IFoodService foods) =>
            await foods.Update(id, view) ? Results.NoContent() : Results.NotFound());

        group.MapDelete("/{id:long}", async (long id, IFoodService foods) =>
            await foods.Delete(id) ? Results.NoContent() : Results.NotFound());

        return app;
    }
}
