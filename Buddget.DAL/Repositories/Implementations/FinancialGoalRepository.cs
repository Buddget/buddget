using Buddget.DAL.DataAccess;
using Buddget.DAL.Entities;
using Buddget.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Buddget.DAL.Repositories.Implementations
{
    public class FinancialGoalRepository : Repository<FinancialGoalEntity>, IFinancialGoalRepository
    {
        private readonly AppDbContext _context;

        public FinancialGoalRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FinancialGoalEntity>> GetGoalsByUserIdAsync(int userId)
        {
            return await _context.FinancialGoals
                .Where(g => g.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<FinancialGoalEntity>> GetGoalsBySpaceIdAsync(int spaceId)
        {
            return await _context.FinancialGoalSpaces
                .Where(gs => gs.FinancialSpaceId == spaceId)
                .Select(gs => gs.FinancialGoal)
                .ToListAsync();
        }
    }
}