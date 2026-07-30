using _40Let.Data;
using _40Let.Models;
using FortyLet.Storage;
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
            food.Image = await ToUrl(food.Image);
        return foods;
    }

    public async Task<Food?> GetById(long id)
    {
        var food = await context.Foods.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
        if (food is not null)
            food.Image = await ToUrl(food.Image);
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

        entity.Image = await ToUrl(entity.Image);
        return entity;
    }

    public async Task<bool> Update(long id, FoodView view)
    {
        var entity = await context.Foods.FirstOrDefaultAsync(f => f.Id == id);
        if (entity is null)
            return false;

        var oldKey = entity.Image;     
        _mapper.Update(view, entity);

        if (view.ImageFile is not null)
        {
            entity.Image = await storage.UploadAsync(view.ImageFile, Folder);
            await context.SaveChangesAsync();

          
            if (!string.IsNullOrEmpty(oldKey) && oldKey != entity.Image)
                await storage.DeleteAsync(oldKey);
        }
        else
        {
            entity.Image = oldKey; 
            await context.SaveChangesAsync();
        }

        return true;
    }

    public async Task<bool> Delete(long id)
    {
        var entity = await context.Foods.FirstOrDefaultAsync(f => f.Id == id);
        if (entity is null)
            return false;

        var key = entity.Image;

        context.Foods.Remove(entity);
        await context.SaveChangesAsync();   // remove the row first

        if (!string.IsNullOrEmpty(key))
            await storage.DeleteAsync(key);  // then the object

        return true;
    }
    #endregion

    private async Task<string> ToUrl(string key)
        => string.IsNullOrEmpty(key) ? key : await storage.GetPresignedUrlAsync(key);
}