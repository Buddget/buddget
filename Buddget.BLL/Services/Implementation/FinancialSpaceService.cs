using AutoMapper;
using Buddget.BLL.DTOs;
using Buddget.BLL.Services.Interfaces;
using Buddget.DAL.Repositories.Interfaces;

namespace Buddget.BLL.Services.Implementations
{
    public class FinancialSpaceService : IFinancialSpaceService
    {
        private readonly IFinancialSpaceRepository _financialSpaceRepository;
        private readonly IFinancialSpaceMemberService _financialSpaceMemberService;
        private readonly IFinancialGoalSpaceService _financialGoalSpaceService;
        private readonly ITransactionService _transactionService;
        private readonly IMapper _mapper;

        public FinancialSpaceService(
            IFinancialSpaceRepository financialSpaceRepository,
            IFinancialSpaceMemberService financialSpaceMemberService,
            IFinancialGoalSpaceService financialGoalSpaceService,
            ITransactionService transactionService,
            IMapper mapper)
        {
            _financialSpaceRepository = financialSpaceRepository;
            _financialSpaceMemberService = financialSpaceMemberService;
            _financialGoalSpaceService = financialGoalSpaceService;
            _transactionService = transactionService;
            _mapper = mapper;
        }

        public async Task<FinancialSpaceDto> GetFinancialSpaceByIdAsync(int spaceId)
        {
            // Отримуємо простір
            var spaceEntity = await _financialSpaceRepository.GetFinancialSpaceAsync(spaceId);
            if (spaceEntity == null)
            {
                return null;
            }

            // Отримуємо членів простору
            var members = await _financialSpaceMemberService.GetMembersBySpaceIdAsync(spaceId);

            // Отримуємо фінансові цілі простору
            var financialGoals = await _financialGoalSpaceService.GetFinancialGoalsBySpaceIdAsync(spaceId);

            // Отримуємо транзакції простору
            var transactions = await _transactionService.GetTransactionsBySpaceIdAsync(spaceId);

            // Мапимо все в DTO
            var spaceDto = _mapper.Map<FinancialSpaceDto>(spaceEntity);
            spaceDto.Goals = (List<FinancialGoalDto>)financialGoals;
            spaceDto.Members = (List<FinancialSpaceMemberDto>)members;
            spaceDto.RecentTransactions = (List<TransactionDto>)transactions;

            return spaceDto;
        }
    }
}
