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
    }
}
