using _40Let.Models;
using Riok.Mapperly.Abstractions;

namespace _40Let.Features;

[Mapper]
public partial class FoodMapper
{
    // Id is DB-generated.
    [MapperIgnoreTarget(nameof(Food.Id))]
    public partial Food ToEntity(FoodView view);

    // Applies the view onto an already-loaded entity (for updates).
    [MapperIgnoreTarget(nameof(Food.Id))]
    public partial void Update(FoodView view, Food entity);
}
