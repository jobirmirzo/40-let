namespace _40Let.Features;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/orders").WithTags("Orders");

        group.MapGet("/", async (IOrderService orders) =>
            Results.Ok(await orders.GetAll()));

        group.MapGet("/{id:long}", async (long id, IOrderService orders) =>
            await orders.GetById(id) is { } order ? Results.Ok(order) : Results.NotFound());

        group.MapPost("/", async (OrderView view, IOrderService orders) =>
        {
            var order = await orders.Create(view);
            return Results.Created($"/orders/{order.Id}", order);
        });

        group.MapPut("/{id:long}", async (long id, OrderView view, IOrderService orders) =>
            await orders.Update(id, view) ? Results.NoContent() : Results.NotFound());

        group.MapDelete("/{id:long}", async (long id, IOrderService orders) =>
            await orders.Delete(id) ? Results.NoContent() : Results.NotFound());

        return app;
    }
}
