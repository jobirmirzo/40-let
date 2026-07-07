using _40Let.Models;
using Riok.Mapperly.Abstractions;

namespace _40Let.Features;

[Mapper]
public partial class BotUserMapper
{
    [MapperIgnoreTarget(nameof(BotUser.Id))]
    [MapperIgnoreTarget(nameof(BotUser.BotUserHistory))]
    public partial BotUser ToEntity(BotUserView view);

    [MapperIgnoreTarget(nameof(BotUser.Id))]
    [MapperIgnoreTarget(nameof(BotUser.BotUserHistory))]
    public partial void Update(BotUserView view, BotUser entity);
}
