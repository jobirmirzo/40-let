using _40Let.Data;
using _40Let.Models;
using Microsoft.EntityFrameworkCore;

namespace _40Let.Features;

public class BotUserService(AppDbContext context) : IBotUserService
{
    private readonly BotUserMapper _mapper = new();

    #region Queries
    public Task<List<BotUser>> GetAll()
        => context.BotUsers.AsNoTracking().ToListAsync();

    public Task<BotUser?> GetById(long id)
        => context.BotUsers.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

    public async Task<BotUser?> GetByChatId(long chatId)
        => await context.BotUsers.AsNoTracking().FirstOrDefaultAsync(u => u.ChatId == chatId);
    
    #endregion

    #region Mutations
    public async Task<BotUser> Create(BotUserView view)
    {
        var entity = _mapper.ToEntity(view);
        await context.BotUsers.AddAsync(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> Update(long id, BotUserView view)
    {
        var entity = await context.BotUsers.FirstOrDefaultAsync(u => u.Id == id);
        if (entity is null)
            return false;

        _mapper.Update(view, entity);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Delete(long id)
    {
        var affected = await context.BotUsers
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync();
        return affected > 0;
    }
    #endregion
}
