using Buddget.Domain.Entities;

namespace Buddget.DAL.Repositories.Interfaces
{
    public interface IFinancialGoalCategoryRepository : IRepository<FinancialGoalCategoryEntity>
    {
        Task<IEnumerable<FinancialGoalCategoryEntity>> GetAllByGoalIdAsync(int goalId);
    }
}