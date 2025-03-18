using Buddget.BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Buddget.BLL.Services.Interfaces
{
    public interface IFinancialSpaceService
    {
        Task<FinancialSpaceDto> GetFinancialSpaceByIdAsync(int spaceId);
        Task<IEnumerable<FinancialSpaceDto>> GetFinancialSpacesUserIsMemberOrOwnerOf(int userId);
        Task<string> DeleteFinancialSpaceAsync(int userId,int id);

    }
}
