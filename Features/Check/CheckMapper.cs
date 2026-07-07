using _40Let.Models;
using Riok.Mapperly.Abstractions;

namespace _40Let.Features;

[Mapper]
public partial class CheckMapper
{
    // Id is DB-generated, CreatedAt defaults in the DB, and the navigations
    // (Order, BotUserHistory) are resolved via the OrderId foreign key.
    [MapperIgnoreTarget(nameof(Check.Id))]
    [MapperIgnoreTarget(nameof(Check.CreatedAt))]
    [MapperIgnoreTarget(nameof(Check.Order))]
    [MapperIgnoreTarget(nameof(Check.BotUserHistory))]
    public partial Check ToEntity(CheckView view);

    // Applies the view onto an already-loaded entity (for updates).
    [MapperIgnoreTarget(nameof(Check.Id))]
    [MapperIgnoreTarget(nameof(Check.CreatedAt))]
    [MapperIgnoreTarget(nameof(Check.Order))]
    [MapperIgnoreTarget(nameof(Check.BotUserHistory))]
    public partial void Update(CheckView view, Check entity);
}
