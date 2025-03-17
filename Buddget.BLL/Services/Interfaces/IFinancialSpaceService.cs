using Buddget.BLL.DTOs;

namespace Buddget.BLL.Services.Interfaces
{
    public interface IFinancialSpaceService
    {
        Task<FinancialSpaceDto> GetFinancialSpaceByIdAsync(int spaceId);
    }
}

