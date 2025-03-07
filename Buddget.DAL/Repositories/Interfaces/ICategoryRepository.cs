using Buddget.DAL.Entities;

namespace Buddget.DAL.Repositories.Interfaces
{
    public interface ICategoryRepository : IRepository<CategoryEntity>
    {
        Task<CategoryEntity> GetDefaultCategories(int id);
        Task<IEnumerable<CategoryEntity>> GetCategoriesByUserIdAsync(int userId);
    }
}