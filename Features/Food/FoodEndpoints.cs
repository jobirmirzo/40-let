using _40Let.Models;
using Microsoft.AspNetCore.Mvc;

namespace _40Let.Features;

public static class FoodEndpoints
{
    public static IEndpointRouteBuilder MapFoodEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/foods").WithTags("Foods");

        group.MapGet("/", async (IFoodService foods) =>
            Results.Ok(await foods.GetAll()))
            .WithName("GetFoods")
            .WithSummary("List all foods")
            .Produces<List<Food>>();

        group.MapGet("/{id:long}", async (long id, IFoodService foods) =>
            await foods.GetById(id) is { } food ? Results.Ok(food) : Results.NotFound())
            .WithName("GetFoodById")
            .WithSummary("Get a food by id")
            .Produces<Food>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async ([FromForm] FoodView view, IFoodService foods) =>
        {
            var food = await foods.Create(view);
            return Results.Created($"/foods/{food.Id}", food);
        })
            .DisableAntiforgery()
            .WithName("CreateFood")
            .WithSummary("Create a food (multipart form; include ImageFile to upload an image)")
            .Accepts<FoodView>("multipart/form-data")
            .Produces<Food>(StatusCodes.Status201Created);

        group.MapPut("/{id:long}", async (long id, [FromForm] FoodView view, IFoodService foods) =>
            await foods.Update(id, view) ? Results.NoContent() : Results.NotFound())
            .DisableAntiforgery()
            .WithName("UpdateFood")
            .WithSummary("Update a food (multipart form; include ImageFile to replace the image)")
            .Accepts<FoodView>("multipart/form-data")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:long}", async (long id, IFoodService foods) =>
            await foods.Delete(id) ? Results.NoContent() : Results.NotFound())
            .WithName("DeleteFood")
            .WithSummary("Delete a food")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}
