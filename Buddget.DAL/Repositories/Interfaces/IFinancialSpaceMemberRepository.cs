using Buddget.Domain.Entities;

namespace Buddget.DAL.Repositories.Interfaces
{
    public interface IFinancialSpaceMemberRepository : IRepository<FinancialSpaceMemberEntity>
    {
        Task<IEnumerable<FinancialSpaceMemberEntity>> GetMembersBySpaceIdAsync(int spaceId);
        Task<IEnumerable<FinancialSpaceMemberEntity>> GetBannedMembersBySpaceIdAsync(int spaceId);
        Task<FinancialSpaceMemberEntity> GetMembershipAsync(int userId, int spaceId);
        Task<bool> IsMemberOfSpaceAsync(int userId, int spaceId);
        Task<FinancialSpaceMemberEntity> CreateAsync(FinancialSpaceMemberEntity entity);
        new Task UpdateAsync(FinancialSpaceMemberEntity entity);
    }
}