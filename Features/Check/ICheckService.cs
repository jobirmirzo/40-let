using _40Let.Models;

namespace _40Let.Features;

public interface ICheckService
{
    #region Queries

    Task<List<Check>> GetAll();
    Task<Check?> GetById(long id);

    #endregion

    #region Mutations

    Task<Check> Create(CheckView view);
    Task<bool> Update(long id, CheckView view);
    Task<bool> Delete(long id);

    #endregion
}
