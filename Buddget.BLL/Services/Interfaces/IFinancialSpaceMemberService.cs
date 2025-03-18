using Buddget.BLL.DTOs;

namespace Buddget.BLL.Services.Interfaces
{
    public interface IFinancialSpaceMemberService
    {
        Task<IEnumerable<FinancialSpaceMemberDto>> GetMembersBySpaceIdAsync(int spaceId);
        Task<CreateFinancialSpaceMemberDto> CreateAsync(CreateFinancialSpaceMemberDto createDto);
        Task InviteMember(string email, int spaceId);
    }
}
