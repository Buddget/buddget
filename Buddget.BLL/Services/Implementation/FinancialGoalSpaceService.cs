using AutoMapper;
using AutoMapper.Execution;
using Buddget.BLL.DTOs;
using Buddget.BLL.Services.Interfaces;
using Buddget.DAL.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buddget.BLL.Services.Implementations
{
    public class FinancialGoalSpaceService : IFinancialGoalSpaceService
    {
        private readonly IFinancialGoalSpaceRepository _financialGoalSpaceRepository;
        private readonly IMapper _mapper;

        public FinancialGoalSpaceService(
            IFinancialGoalSpaceRepository financialGoalSpaceRepository,
            IMapper mapper)
        {
            _financialGoalSpaceRepository = financialGoalSpaceRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FinancialGoalDto>> GetFinancialGoalsBySpaceIdAsync(int spaceId)
        {
            var financialGoals = await _financialGoalSpaceRepository.GetAllBySpaceIdAsync(spaceId);
            return _mapper.Map<IEnumerable<FinancialGoalDto>>(financialGoals);
        }
    }
}
