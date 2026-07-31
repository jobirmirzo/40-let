using _40Let.Enum;
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
    Task<bool> UpdateRole(long id, Role role);
    Task<bool> Delete(long id);

    /// <summary>
    /// Gets or creates the BotUser for a superadmin's chat id, promoting it to
    /// Role.SuperAdmin if it already exists under some other role (e.g. it was
    /// created earlier by the plain contact-share flow, or before this chat id
    /// was added to SuperAdmin:ChatIds). Called on every /start so superadmin
    /// status is self-healing rather than a one-time stamp.
    /// </summary>
    Task<BotUser> EnsureSuperAdmin(long chatId, string? fullname);

    #endregion
}
