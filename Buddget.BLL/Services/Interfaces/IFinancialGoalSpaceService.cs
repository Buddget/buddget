using Buddget.BLL.DTOs;

namespace Buddget.BLL.Services.Interfaces
{
    public interface IFinancialGoalSpaceService
    {
        Task<IEnumerable<FinancialGoalDto>> GetFinancialGoalsBySpaceIdAsync(int spaceId);
    }
}
