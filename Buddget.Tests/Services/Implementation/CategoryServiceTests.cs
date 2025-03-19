using AutoMapper;
using Buddget.BLL.DTOs;
using Buddget.BLL.Services.Implementations;
using Buddget.DAL.Entities;
using Buddget.DAL.Repositories.Interfaces;
using Moq;

namespace Buddget.Tests.Services.Implementation
{
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly IMapper _mapper;
        private readonly CategoryService _categoryService;

        public CategoryServiceTests()
        {
            _mockCategoryRepository = new Mock<ICategoryRepository>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CategoryEntity, CategoryDto>().ReverseMap();
            });
            _mapper = mapperConfig.CreateMapper();

            _categoryService = new CategoryService(_mockCategoryRepository.Object, _mapper);
        }

        [Fact]
        public async Task AddCustomCategoryAsync_SuccessfullyAddsCategory()
        {
            // Arrange
            int userId = 1;
            string categoryName = "New Category";
            _mockCategoryRepository.Setup(repo => repo.GetFirstWhereAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<CategoryEntity, bool>>>()))
                .ReturnsAsync((CategoryEntity)null);

            // Act
            var result = await _categoryService.AddCustomCategoryAsync(userId, categoryName);

            // Assert
            Assert.True(result);
            _mockCategoryRepository.Verify(repo => repo.AddAsync(It.Is<CategoryEntity>(c => c.UserId == userId && c.Name == categoryName)), Times.Once);
        }

        [Fact]
        public async Task AddCustomCategoryAsync_CategoryAlreadyExists_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            string categoryName = "Existing Category";
            var existingCategory = new CategoryEntity { UserId = userId, Name = categoryName };
            _mockCategoryRepository.Setup(repo => repo.GetFirstWhereAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<CategoryEntity, bool>>>()))
                .ReturnsAsync(existingCategory);

            // Act
            var result = await _categoryService.AddCustomCategoryAsync(userId, categoryName);

            // Assert
            Assert.False(result);
            _mockCategoryRepository.Verify(repo => repo.AddAsync(It.IsAny<CategoryEntity>()), Times.Never);
        }

        [Fact]
        public async Task GetCustomCategoriesByUserIdAsync_ReturnsCategories()
        {
            // Arrange
            int userId = 1;
            var categories = new List<CategoryEntity>
            {
                new CategoryEntity { Id = 1, UserId = userId, Name = "Category 1" },
                new CategoryEntity { Id = 2, UserId = userId, Name = "Category 2" }
            };
            _mockCategoryRepository.Setup(repo => repo.GetCategoriesByUserIdAsync(userId)).ReturnsAsync(categories);

            // Act
            var result = await _categoryService.GetCustomCategoriesByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("Category 1", result.First().Name);
            Assert.Equal("Category 2", result.Last().Name);
        }

        [Fact]
        public async Task GetCustomCategoriesByUserIdAsync_ReturnsEmptyList_WhenNoCategoriesFound()
        {
            // Arrange
            int userId = 1;
            _mockCategoryRepository.Setup(repo => repo.GetCategoriesByUserIdAsync(userId)).ReturnsAsync(new List<CategoryEntity>());

            // Act
            var result = await _categoryService.GetCustomCategoriesByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task DeleteCustomCategoryAsync_SuccessfullyDeletesCategory()
        {
            // Arrange
            int userId = 1;
            int categoryId = 1;
            var category = new CategoryEntity { Id = categoryId, UserId = userId, Name = "Category to Delete" };
            _mockCategoryRepository.Setup(repo => repo.GetFirstWhereAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<CategoryEntity, bool>>>()))
                .ReturnsAsync(category);

            // Act
            var result = await _categoryService.DeleteCustomCategoryAsync(userId, categoryId);

            // Assert
            Assert.True(result);
            _mockCategoryRepository.Verify(repo => repo.DeleteAsync(It.Is<CategoryEntity>(c => c.Id == categoryId && c.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task DeleteCustomCategoryAsync_CategoryNotFound_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            int categoryId = 1;
            _mockCategoryRepository.Setup(repo => repo.GetFirstWhereAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<CategoryEntity, bool>>>()))
                .ReturnsAsync((CategoryEntity)null);

            // Act
            var result = await _categoryService.DeleteCustomCategoryAsync(userId, categoryId);

            // Assert
            Assert.False(result);
            _mockCategoryRepository.Verify(repo => repo.DeleteAsync(It.IsAny<CategoryEntity>()), Times.Never);
        }
    }
}