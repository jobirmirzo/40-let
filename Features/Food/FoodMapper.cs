using _40Let.Models;
using Riok.Mapperly.Abstractions;

namespace _40Let.Features;

[Mapper]
public partial class FoodMapper
{
    // Id is DB-generated; Image is the MinIO object key managed by MinioService,
    // and ImageFile is the raw upload consumed by the service (never mapped here).
    [MapperIgnoreTarget(nameof(Food.Id))]
    [MapperIgnoreTarget(nameof(Food.Image))]
    [MapperIgnoreSource(nameof(FoodView.ImageFile))]
    public partial Food ToEntity(FoodView view);

    // Applies the view onto an already-loaded entity (for updates).
    [MapperIgnoreTarget(nameof(Food.Id))]
    [MapperIgnoreTarget(nameof(Food.Image))]
    [MapperIgnoreSource(nameof(FoodView.ImageFile))]
    public partial void Update(FoodView view, Food entity);
}
