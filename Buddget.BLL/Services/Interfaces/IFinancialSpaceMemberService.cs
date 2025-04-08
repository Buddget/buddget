using Buddget.BLL.DTOs;

namespace Buddget.BLL.Services.Interfaces
{
    public interface IFinancialSpaceMemberService
    {
        Task<IEnumerable<FinancialSpaceMemberDto>> GetMembersBySpaceIdAsync(int spaceId);
        Task<IEnumerable<FinancialSpaceMemberDto>> GetBannedMembersBySpaceIdAsync(int spaceId);
        Task<string> BanMemberAsync(int spaceId, int memberId, int requestingUserId);
        Task InviteMember(string email, int spaceId);
        Task<FinancialSpaceMemberDto> CreateAsync(CreateFinancialSpaceMemberDto createDto);
    }
}
