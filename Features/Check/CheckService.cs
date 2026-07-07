using _40Let.Data;
using _40Let.Models;
using Microsoft.EntityFrameworkCore;

namespace _40Let.Features;

public class CheckService(AppDbContext context) : ICheckService
{
    private readonly CheckMapper _mapper = new();

    #region Queries
    public Task<List<Check>> GetAll()
        => context.Checks.AsNoTracking().ToListAsync();

    public Task<Check?> GetById(long id)
        => context.Checks.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
    #endregion

    #region Mutations
    public async Task<Check> Create(CheckView view)
    {
        var entity = _mapper.ToEntity(view);
        await context.Checks.AddAsync(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> Update(long id, CheckView view)
    {
        var entity = await context.Checks.FirstOrDefaultAsync(c => c.Id == id);
        if (entity is null)
            return false;

        _mapper.Update(view, entity);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Delete(long id)
    {
        var affected = await context.Checks
            .Where(c => c.Id == id)
            .ExecuteDeleteAsync();
        return affected > 0;
    }
    #endregion
}
