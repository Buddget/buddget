using AutoMapper;
using Buddget.BLL.DTOs;
using Buddget.BLL.Services.Interfaces;
using Buddget.DAL.Repositories.Interfaces;

namespace Buddget.BLL.Services.Implementation
{
    public class FinancialSpaceMemberService : IFinancialSpaceMemberService
    {
        private readonly IFinancialSpaceMemberRepository _financialSpaceMemberRepository;
        private readonly IMapper _mapper;

        public FinancialSpaceMemberService(
            IFinancialSpaceMemberRepository financialSpaceMemberRepository,
            IMapper mapper)
        {
            _financialSpaceMemberRepository = financialSpaceMemberRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FinancialSpaceMemberDto>> GetMembersBySpaceIdAsync(int spaceId)
        {
            var members = await _financialSpaceMemberRepository.GetMembersBySpaceIdAsync(spaceId);
            var memberDtos = new List<FinancialSpaceMemberDto>();

            foreach (var member in members)
            {
                var memberDto = new FinancialSpaceMemberDto
                {
                    Id = member.User?.Id ?? 0,
                    FirstName = member.User?.FirstName ?? "N/A",
                    LastName = member.User?.LastName ?? "N/A",
                    Email = member.User?.Email ?? "N/A",
                    RegisteredAt = member.User?.RegisteredAt ?? DateTime.MinValue,
                    Role = member.Role,
                    FinancialSpaceId = member.FinancialSpaceId,
                    MemberRole = member.Role,
                };

                memberDtos.Add(memberDto);
            }

            return memberDtos;
        }

        public async Task<IEnumerable<FinancialSpaceMemberDto>> GetBannedMembersBySpaceIdAsync(int spaceId)
        {
            var members = await _financialSpaceMemberRepository.GetBannedMembersBySpaceIdAsync(spaceId);
            var memberDtos = new List<FinancialSpaceMemberDto>();
            foreach (var member in members)
            {
                var memberDto = new FinancialSpaceMemberDto
                {
                    Id = member.User?.Id ?? 0,
                    FirstName = member.User?.FirstName ?? "N/A",
                    LastName = member.User?.LastName ?? "N/A",
                    Email = member.User?.Email ?? "N/A",
                    RegisteredAt = member.User?.RegisteredAt ?? DateTime.MinValue,
                    Role = member.Role,
                    FinancialSpaceId = member.FinancialSpaceId,
                    MemberRole = member.Role,
                };
                memberDtos.Add(memberDto);
            }

            return memberDtos;
        }
        public async Task<string> BanMemberAsync(int spaceId, int memberId, int requestingUserId)
        {
            try
            {
                var members = await _financialSpaceMemberRepository.GetMembersBySpaceIdAsync(spaceId);

                var owner = members.FirstOrDefault(m => m.Role == "Owner");

                if (owner == null || owner.UserId != requestingUserId)
                {
                    return "Only the owner of the financial space can ban members.";
                }
                var memberToBan = members.FirstOrDefault(m => m.UserId == memberId);

                if (memberToBan == null)
                {
                    return "Member not found in this financial space.";
                }

                if (memberToBan.Role == "Owner")
                {
                    return "Cannot ban the owner of the financial space.";
                }

                var bannedMembers = await _financialSpaceMemberRepository.GetBannedMembersBySpaceIdAsync(spaceId);
                if (bannedMembers.Any(m => m.UserId == memberId))
                {
                    return "Member is already banned from this financial space.";
                }

                memberToBan.Role = "Banned";

                try
                {
                    await _financialSpaceMemberRepository.UpdateAsync(memberToBan);
                    return "Member has been successfully banned from the financial space.";
                }
                catch
                {
                    return "Failed to ban member. Please try again later.";
                }

            }
            catch (Exception ex)
            {
                return $"An error occurred while banning the member: {ex.Message}";
            }
        }
    }
}
