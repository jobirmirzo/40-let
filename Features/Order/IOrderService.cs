using _40Let.Models;

namespace _40Let.Features;

public interface IOrderService
{
    #region Queries

    Task<List<Order>> GetAll();
    Task<Order?> GetById(long id);

    #endregion

    #region Mutations

    Task<Order> Create(OrderView view);
    Task<bool> Update(long id, OrderView view);
    Task<bool> Delete(long id);

    #endregion
}
