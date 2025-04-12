using Buddget.Domain.Entities;

namespace Buddget.DAL.Repositories.Interfaces
{
    public interface IFinancialGoalSpaceRepository : IRepository<FinancialGoalSpaceEntity>
    {
        Task<IEnumerable<FinancialGoalSpaceEntity>> GetAllBySpaceIdAsync(int spaceId);
    }
}