using _40Let.Models;

namespace _40Let.Features;

public interface IFoodService
{
    #region Queries

    Task<List<Food>> GetAll();
    Task<Food?> GetById(long id);

    #endregion

    #region Mutations

    Task<Food> Create(FoodView view);
    Task<bool> Update(long id, FoodView view);
    Task<bool> Delete(long id);

    #endregion
}
