using Buddget.DAL.DataAccess;
using Buddget.DAL.Entities;
using Buddget.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Buddget.DAL.Repositories.Implementations
{
    public class CategoryRepository : Repository<CategoryEntity>, ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
            : base(context)
        {
            this._context = context;
        }

        public async Task<IEnumerable<CategoryEntity>> GetCategoriesByUserIdAsync(int userId)
        {
            return await _context.Categories
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<CategoryEntity> GetDefaultCategories(int id)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.UserId == id && c.IsDefault);
        }
    }
}