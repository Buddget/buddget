using Buddget.DAL.DataAccess;
using Buddget.DAL.Entities;
using Buddget.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Buddget.DAL.Repositories.Implementations
{
    public class FinancialGoalCategoryRepository : Repository<FinancialGoalCategoryEntity>, IFinancialGoalCategoryRepository
    {
        private readonly AppDbContext _context;

        public FinancialGoalCategoryRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FinancialGoalCategoryEntity>> GetAllByGoalIdAsync(int goalId)
        {
            return await _context.FinancialGoalCategories
                .Where(gc => gc.FinancialGoalId == goalId)
                .Include(gc => gc.Category)
                .ToListAsync();
        }
    }
}