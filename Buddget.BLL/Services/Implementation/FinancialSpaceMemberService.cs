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
            return _mapper.Map<IEnumerable<FinancialSpaceMemberDto>>(members);
        }
    }
}
