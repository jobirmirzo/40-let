using _40Let.Models;
using Riok.Mapperly.Abstractions;

namespace _40Let.Features;

[Mapper]
public partial class OrderMapper
{
    // Id is DB-generated and CreatedAt defaults in the DB. Items come along via
    // the OrderItemView mapping below, so the whole basket saves in one call.
    [MapperIgnoreTarget(nameof(Order.Id))]
    [MapperIgnoreTarget(nameof(Order.CreatedAt))]
    public partial Order ToEntity(OrderView view);

    // Applies the view onto an already-loaded entity (for updates). Items are
    // replaced by the service, which has to delete the old rows first.
    [MapperIgnoreTarget(nameof(Order.Id))]
    [MapperIgnoreTarget(nameof(Order.CreatedAt))]
    [MapperIgnoreTarget(nameof(Order.Items))]
    [MapperIgnoreSource(nameof(OrderView.Items))]
    public partial void Update(OrderView view, Order entity);

    // OrderId is set by EF from the parent; navigations are never mapped.
    [MapperIgnoreTarget(nameof(OrderItem.Id))]
    [MapperIgnoreTarget(nameof(OrderItem.OrderId))]
    [MapperIgnoreTarget(nameof(OrderItem.Order))]
    [MapperIgnoreTarget(nameof(OrderItem.Food))]
    public partial OrderItem ToEntity(OrderItemView view);
}
