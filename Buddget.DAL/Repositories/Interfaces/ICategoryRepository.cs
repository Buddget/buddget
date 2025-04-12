using Buddget.Domain.Entities;

namespace Buddget.DAL.Repositories.Interfaces
{
    public interface ICategoryRepository : IRepository<CategoryEntity>
    {
        Task<IEnumerable<CategoryEntity>> GetDefaultCategoriesAsync();
        Task<IEnumerable<CategoryEntity>> GetCategoriesByUserIdAsync(int userId);
    }
}