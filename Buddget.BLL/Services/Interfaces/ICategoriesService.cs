using Buddget.BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Buddget.BLL.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetCustomCategoriesByUserIdAsync(int userId);
    }
}