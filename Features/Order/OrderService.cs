using _40Let.Data;
using _40Let.Models;
using Microsoft.EntityFrameworkCore;

namespace _40Let.Features;

public class OrderService(AppDbContext context) : IOrderService
{
    private readonly OrderMapper _mapper = new();

    #region Queries
    public Task<List<Order>> GetAll()
        => context.Orders.AsNoTracking().Include(o => o.Items).ToListAsync();

    public Task<Order?> GetById(long id)
        => context.Orders.AsNoTracking().Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);
    #endregion

    #region Mutations
    public async Task<Order> Create(OrderView view)
    {
        var entity = _mapper.ToEntity(view);
        await context.Orders.AddAsync(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> Update(long id, OrderView view)
    {
        var entity = await context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (entity is null)
            return false;

        _mapper.Update(view, entity);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Delete(long id)
    {
        var affected = await context.Orders
            .Where(o => o.Id == id)
            .ExecuteDeleteAsync();
        return affected > 0;
    }
    #endregion
}
