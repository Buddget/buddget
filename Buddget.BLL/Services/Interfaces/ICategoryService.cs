using Buddget.BLL.DTOs;

namespace Buddget.BLL.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetCustomCategoriesByUserIdAsync(int userId);
        Task<bool> AddCustomCategoryAsync(int userId, string categoryName);
    }
}