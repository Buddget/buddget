using Buddget.Domain.Entities;

namespace Buddget.DAL.Repositories.Interfaces
{
    public interface IFinancialGoalRepository : IRepository<FinancialGoalEntity>
    {
        Task<IEnumerable<FinancialGoalEntity>> GetGoalsByUserIdAsync(int userId);
        Task<IEnumerable<FinancialGoalEntity>> GetGoalsBySpaceIdAsync(int spaceId);
    }
}