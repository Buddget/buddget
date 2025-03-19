using AutoMapper;
using Buddget.BLL.DTOs;
using Buddget.BLL.Services.Interfaces;
using Buddget.DAL.Entities;
using Buddget.DAL.Repositories.Interfaces;

namespace Buddget.BLL.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetCustomCategoriesByUserIdAsync(int userId)
        {
            var categories = await _categoryRepository.GetCategoriesByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<bool> AddCustomCategoryAsync(int userId, string categoryName)
        {
            var existingCategory = await _categoryRepository.GetFirstWhereAsync(c => c.UserId == userId && c.Name == categoryName);
            if (existingCategory != null)
            {
                return false;
            }

            var newCategory = new CategoryEntity
            {
                UserId = userId,
                Name = categoryName,
                IsDefault = false,
            };

            await _categoryRepository.AddAsync(newCategory);
            return true;
        }

        public async Task<bool> DeleteCustomCategoryAsync(int userId, int categoryId)
        {
            var category = await _categoryRepository.GetFirstWhereAsync(c => c.UserId == userId && c.Id == categoryId);
            if (category == null)
            {
                return false;
            }

            await _categoryRepository.DeleteAsync(category);
            return true;
        }
    }
}
