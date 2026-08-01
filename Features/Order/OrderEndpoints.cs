using System.Security.Claims;
using _40Let.Extensions;
using _40Let.Models;
using Microsoft.AspNetCore.Mvc;

namespace _40Let.Features;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/orders").WithTags("Orders");

        group.MapGet("/", async (ClaimsPrincipal principal, IOrderService orders) =>
        {
            var userId = principal.GetUserId();
            if (userId is null)
                return Results.Unauthorized();

            return Results.Ok(await orders.GetAll(userId.Value));
        })
            .RequireAuthorization()
            .WithName("GetOrders")
            .WithSummary("List the current user's orders")
            .Produces<List<Order>>()
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/{id:long}", async (long id, IOrderService orders) =>
            await orders.GetById(id) is { } order ? Results.Ok(order) : Results.NotFound())
            .WithName("GetOrderById")
            .WithSummary("Get an order by id")
            .Produces<Order>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async (OrderView view, ClaimsPrincipal principal, IOrderService orders) =>
        {
            var userId = principal.GetUserId();
            if (userId is null)
                return Results.Unauthorized();

            var order = await orders.Create(view, userId.Value);
            return Results.Created($"/orders/{order.Id}", order);
        })
            .RequireAuthorization()
            .WithName("CreateOrder")
            .WithSummary("Create an order for the current user")
            .Produces<Order>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPut("/{id:long}", async (long id, OrderView view, IOrderService orders) =>
            await orders.Update(id, view) ? Results.NoContent() : Results.NotFound())
            .WithName("UpdateOrder")
            .WithSummary("Update an order")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPatch("/{id:long}/status", async (long id, OrderStatusView view, IOrderService orders) =>
        {
            if (!System.Enum.IsDefined(view.Status))
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    [nameof(view.Status)] = [$"'{(int)view.Status}' is not a valid order status."]
                });

            return await orders.UpdateStatus(id, view.Status) ? Results.NoContent() : Results.NotFound();
        })
            .WithName("UpdateOrderStatus")
            .WithSummary("Change an order's status")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        group.MapDelete("/{id:long}", async (long id, IOrderService orders) =>
            await orders.Delete(id) ? Results.NoContent() : Results.NotFound())
            .WithName("DeleteOrder")
            .WithSummary("Delete an order")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}
