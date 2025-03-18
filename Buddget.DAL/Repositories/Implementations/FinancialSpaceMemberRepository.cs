using Buddget.DAL.DataAccess;
using Buddget.DAL.Entities;
using Buddget.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Buddget.DAL.Repositories.Implementations
{
    public class FinancialSpaceMemberRepository : Repository<FinancialSpaceMemberEntity>, IFinancialSpaceMemberRepository
    {
        private readonly AppDbContext _context;

        public FinancialSpaceMemberRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FinancialSpaceMemberEntity>> GetMembersBySpaceIdAsync(int spaceId)
        {
            return await _context.FinancialSpaceMembers
                .Include(m => m.User) // Ensure the User entity is included
                .Where(m => m.FinancialSpaceId == spaceId)
                .ToListAsync();
        }

        public async Task<FinancialSpaceMemberEntity> GetMembershipAsync(int userId, int spaceId)
        {
            return await _context.FinancialSpaceMembers
                .FirstOrDefaultAsync(m => m.UserId == userId && m.FinancialSpaceId == spaceId);
        }

        public async Task<bool> IsMemberOfSpaceAsync(int userId, int spaceId)
        {
            return await _context.FinancialSpaceMembers
                .AnyAsync(m => m.UserId == userId && m.FinancialSpaceId == spaceId);
        }
    }
}