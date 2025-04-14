using Buddget.DAL.DataAccess;
using Buddget.DAL.Repositories.Interfaces;
using Buddget.Domain.Entities;
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
                .Where(m => m.FinancialSpaceId == spaceId && m.Role != "Banned")
                .ToListAsync();
        }

        public async Task<IEnumerable<FinancialSpaceMemberEntity>> GetBannedMembersBySpaceIdAsync(int spaceId)
        {
            return await _context.FinancialSpaceMembers
                .Include(m => m.User) // Ensure the User entity is included
                .Where(m => m.FinancialSpaceId == spaceId && m.Role == "Banned")
                .ToListAsync();
        }

        public async Task<FinancialSpaceMemberEntity> GetMembershipAsync(int userId, int spaceId)
        {
            return await _context.FinancialSpaceMembers
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.UserId == userId && m.FinancialSpaceId == spaceId);
        }

        public async Task<bool> IsMemberOfSpaceAsync(int userId, int spaceId)
        {
            return await _context.FinancialSpaceMembers
                .AnyAsync(m => m.UserId == userId && m.FinancialSpaceId == spaceId);
        }

        public async Task<FinancialSpaceMemberEntity> CreateAsync(FinancialSpaceMemberEntity entity)
        {
            // First, ensure we have the User entity loaded
            var user = await _context.Users.FindAsync(entity.UserId);
            entity.User = user;

            _context.FinancialSpaceMembers.Add(entity);
            await _context.SaveChangesAsync();

            // Reload the entity with the User navigation property
            return await _context.FinancialSpaceMembers
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == entity.Id);
        }
    }
}