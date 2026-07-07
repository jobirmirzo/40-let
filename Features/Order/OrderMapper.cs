using _40Let.Models;
using Riok.Mapperly.Abstractions;

namespace _40Let.Features;

[Mapper]
public partial class OrderMapper
{
    // Id is DB-generated, CreatedAt defaults in the DB, and Items are added separately.
    [MapperIgnoreTarget(nameof(Order.Id))]
    [MapperIgnoreTarget(nameof(Order.CreatedAt))]
    [MapperIgnoreTarget(nameof(Order.Items))]
    public partial Order ToEntity(OrderView view);

    // Applies the view onto an already-loaded entity (for updates).
    [MapperIgnoreTarget(nameof(Order.Id))]
    [MapperIgnoreTarget(nameof(Order.CreatedAt))]
    [MapperIgnoreTarget(nameof(Order.Items))]
    public partial void Update(OrderView view, Order entity);
}
