using _40Let.Models;

namespace _40Let.Features;

public interface IBotUserService
{
    #region Queries

    Task<List<BotUser>> GetAll();
    Task<BotUser?> GetById(long id);
    Task<BotUser?> GetByChatId(long chatId); 

    #endregion

    #region Mutations

    Task<BotUser> Create(BotUserView view);
    Task<bool> Update(long id, BotUserView view);
    Task<bool> Delete(long id);

    #endregion
}
