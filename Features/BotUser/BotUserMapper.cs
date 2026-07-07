using _40Let.Models;
using Riok.Mapperly.Abstractions;

namespace _40Let.Features;

[Mapper]
public partial class BotUserMapper
{
    // Id is DB-generated and BotUserHistory is a navigation collection,
    // so neither is populated from the incoming view.
    [MapperIgnoreTarget(nameof(BotUser.Id))]
    [MapperIgnoreTarget(nameof(BotUser.BotUserHistory))]
    public partial BotUser ToEntity(BotUserView view);

    // Applies the view onto an already-loaded entity (for updates).
    [MapperIgnoreTarget(nameof(BotUser.Id))]
    [MapperIgnoreTarget(nameof(BotUser.BotUserHistory))]
    public partial void Update(BotUserView view, BotUser entity);
}
