using Buddget.BLL.DTOs;

namespace Buddget.BLL.Services.Interfaces
{
    public interface IFinancialSpaceService
    {
        Task<FinancialSpaceDto> GetFinancialSpaceByIdAsync(int spaceId);
        Task<IEnumerable<FinancialSpaceDto>> GetFinancialSpacesUserIsMemberOrOwnerOf(int userId);
        Task<string> DeleteFinancialSpaceAsync(int userId, int id);
    }
}
