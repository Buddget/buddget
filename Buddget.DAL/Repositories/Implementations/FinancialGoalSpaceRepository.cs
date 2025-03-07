using Buddget.DAL.DataAccess;
using Buddget.DAL.Entities;
using Buddget.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Buddget.DAL.Repositories.Implementations
{
    public class FinancialGoalSpaceRepository : Repository<FinancialGoalSpaceEntity>, IFinancialGoalSpaceRepository
    {
        private readonly AppDbContext _context;

        public FinancialGoalSpaceRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FinancialGoalSpaceEntity>> GetAllBySpaceIdAsync(int spaceId)
        {
            return await _context.FinancialGoalSpaces
                .Where(gs => gs.FinancialSpaceId == spaceId)
                .Include(gs => gs.FinancialGoal)
                .ToListAsync();
        }

    }
}