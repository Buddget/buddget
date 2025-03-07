using Buddget.DAL.Entities;

namespace Buddget.DAL.Repositories.Interfaces
{
    public interface IFinancialSpaceMemberRepository : IRepository<FinancialSpaceMemberEntity>
    {
        Task<IEnumerable<FinancialSpaceMemberEntity>> GetMembersBySpaceIdAsync(int spaceId);
        Task<FinancialSpaceMemberEntity> GetMembershipAsync(int userId, int spaceId);
        Task<bool> IsMemberOfSpaceAsync(int userId, int spaceId);
    }
}