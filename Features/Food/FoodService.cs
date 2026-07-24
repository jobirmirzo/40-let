using _40Let.Data;
using _40Let.Models;
using Microsoft.EntityFrameworkCore;

namespace _40Let.Features;

public class FoodService(AppDbContext context, IMinioService storage) : IFoodService
{
    private const string Folder = "foods";
    private readonly FoodMapper _mapper = new();

    #region Queries
    public async Task<List<Food>> GetAll()
    {
        var foods = await context.Foods.AsNoTracking().ToListAsync();
        foreach (var food in foods)
            food.Image = await storage.GetPresignedUrlAsync(food.Image);
        return foods;
    }

    public async Task<Food?> GetById(long id)
    {
        var food = await context.Foods.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
        if (food is not null)
            food.Image = await storage.GetPresignedUrlAsync(food.Image);
        return food;
    }
    #endregion

    #region Mutations
    public async Task<Food> Create(FoodView view)
    {
        var entity = _mapper.ToEntity(view);

        if (view.ImageFile is not null)
            entity.Image = await storage.UploadAsync(view.ImageFile, Folder);

        await context.Foods.AddAsync(entity);
        await context.SaveChangesAsync();

        entity.Image = await storage.GetPresignedUrlAsync(entity.Image);
        return entity;
    }

    public async Task<bool> Update(long id, FoodView view)
    {
        var entity = await context.Foods.FirstOrDefaultAsync(f => f.Id == id);
        if (entity is null)
            return false;

        _mapper.Update(view, entity);

        // Only replace the stored image when a new file is supplied.
        if (view.ImageFile is not null)
        {
            var oldKey = entity.Image;
            entity.Image = await storage.UploadAsync(view.ImageFile, Folder);
            await storage.DeleteAsync(oldKey);
        }

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Delete(long id)
    {
        var entity = await context.Foods.FirstOrDefaultAsync(f => f.Id == id);
        if (entity is null)
            return false;

        await storage.DeleteAsync(entity.Image);
        context.Foods.Remove(entity);
        await context.SaveChangesAsync();
        return true;
    }
    #endregion
}
