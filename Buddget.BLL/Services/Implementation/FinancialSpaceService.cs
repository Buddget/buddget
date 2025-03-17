using AutoMapper;
using Buddget.BLL.DTOs;
using Buddget.BLL.Services.Interfaces;
using Buddget.DAL.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buddget.BLL.Services.Implementations
{
    public class FinancialSpaceService : IFinancialSpaceService
    {
        private readonly IFinancialSpaceRepository _financialSpaceRepository;
        private readonly IFinancialSpaceMemberRepository _financialSpaceMemberRepository;
        private readonly IFinancialGoalSpaceRepository _financialGoalSpaceRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;

        public FinancialSpaceService(
            IFinancialSpaceRepository financialSpaceRepository,
            IFinancialSpaceMemberRepository financialSpaceMemberRepository,
            IFinancialGoalSpaceRepository financialGoalSpaceRepository,
            ITransactionRepository transactionRepository,
            IMapper mapper)
        {
            _financialSpaceRepository = financialSpaceRepository;
            _financialSpaceMemberRepository = financialSpaceMemberRepository;
            _financialGoalSpaceRepository = financialGoalSpaceRepository;
            _transactionRepository = transactionRepository;
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
            var members = await _financialSpaceMemberRepository.GetMembersBySpaceIdAsync(spaceId);
            // Отримуємо фінансові цілі простору
            var financialGoals = await _financialGoalSpaceRepository.GetAllBySpaceIdAsync(spaceId);
            // Отримуємо транзакції простору
            var transactions = await _transactionRepository.GetTransactionsBySpaceIdAsync(spaceId);

            // Мапимо все в DTO
            var spaceDto = _mapper.Map<FinancialSpaceDto>(spaceEntity);
            //spaceDto.Members = members.Select(m => _mapper.Map<UserDto>(m.User)).ToList();
            spaceDto.Goals = financialGoals.Select(fg => _mapper.Map<FinancialGoalDto>(fg.FinancialGoal)).ToList();
            //spaceDto.RecentTransactions = transactions.Select(t => _mapper.Map<TransactionDto>(t)).ToList();

            return spaceDto;
        }
    }
}
