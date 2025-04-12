using Buddget.DAL.DataAccess;
using Buddget.DAL.Repositories.Interfaces;
using Buddget.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Buddget.DAL.Repositories.Implementations
{
    public class CategoryRepository : Repository<CategoryEntity>, ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryEntity>> GetCategoriesByUserIdAsync(int userId)
        {
            return await _context.Categories
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<CategoryEntity>> GetDefaultCategoriesAsync()
{
    return await _context.Categories
        .Where(c => c.IsDefault == true)
        .ToListAsync();
}
    }
}