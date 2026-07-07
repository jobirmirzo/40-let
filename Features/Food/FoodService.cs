using _40Let.Data;
using _40Let.Models;
using Microsoft.EntityFrameworkCore;

namespace _40Let.Features;

public class FoodService(AppDbContext context) : IFoodService
{
    private readonly FoodMapper _mapper = new();

    #region Queries
    public Task<List<Food>> GetAll()
        => context.Foods.AsNoTracking().ToListAsync();

    public Task<Food?> GetById(long id)
        => context.Foods.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
    #endregion

    #region Mutations
    public async Task<Food> Create(FoodView view)
    {
        var entity = _mapper.ToEntity(view);
        await context.Foods.AddAsync(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> Update(long id, FoodView view)
    {
        var entity = await context.Foods.FirstOrDefaultAsync(f => f.Id == id);
        if (entity is null)
            return false;

        _mapper.Update(view, entity);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Delete(long id)
    {
        var affected = await context.Foods
            .Where(f => f.Id == id)
            .ExecuteDeleteAsync();
        return affected > 0;
    }
    #endregion
}
