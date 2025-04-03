using Buddget.DAL.DataAccess;
using Buddget.DAL.Entities;
using Buddget.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Buddget.DAL.Repositories.Implementations
{
    public class FinancialSpaceRepository : Repository<FinancialSpaceEntity>, IFinancialSpaceRepository
    {
        private readonly AppDbContext _context;

        public FinancialSpaceRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FinancialSpaceEntity>> GetSpacesUserIsOwnerOfAsync(int userId)
        {
            return await _context.FinancialSpaces
                .Where(s => s.OwnerId == userId)
                .Include(s => s.Owner)
                .ToListAsync();
        }

        public async Task<IEnumerable<FinancialSpaceEntity>> GetSpacesUserIsMemberOfAsync(int userId)
        {
            return await _context.FinancialSpaceMembers
                .Where(m => m.UserId == userId)
                .Include(m => m.FinancialSpace)
                    .ThenInclude(fs => fs.Owner)
                .Select(m => m.FinancialSpace)
                .ToListAsync();
        }

        public async Task<IEnumerable<FinancialSpaceEntity>> GetSpacesUserIsMemberOrOwnerOf(int userId)
        {
            var spacesOwned = await GetSpacesUserIsOwnerOfAsync(userId);
            var spacesMember = await GetSpacesUserIsMemberOfAsync(userId);
            return spacesOwned.Concat(spacesMember).Distinct();
        }

        public async Task<IEnumerable<FinancialSpaceEntity>> GetSpacesUserIsBannedIn(int userId)
        {
            return await _context.FinancialSpaceMembers
                .Where(b => b.UserId == userId && b.Role == "banned")
                .Select(b => b.FinancialSpace)
                .ToListAsync();
        }

        public async Task<FinancialSpaceEntity> GetFinancialSpaceAsync(int id)
        {
            var space = await _context.FinancialSpaces
                .Include(fs => fs.Owner)
                .Include(fs => fs.Members)
                .FirstOrDefaultAsync(fs => fs.Id == id);

            return space;
        }
    }
}