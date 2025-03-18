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
            // Get financial space
            var spaceEntity = await _financialSpaceRepository.GetFinancialSpaceAsync(spaceId);
            if (spaceEntity == null)
            {
                return null;
            }

            // Get members of the space
            var members = await _financialSpaceMemberService.GetMembersBySpaceIdAsync(spaceId);

            // Get financial goals of the space
            var financialGoals = await _financialGoalSpaceService.GetFinancialGoalsBySpaceIdAsync(spaceId);

            // Get transactions of the space
            var transactions = await _transactionService.GetTransactionsBySpaceIdAsync(spaceId);

            // Map to DTO
            var spaceDto = _mapper.Map<FinancialSpaceDto>(spaceEntity);
            spaceDto.OwnerName = spaceEntity.Owner.FirstName + " " + spaceEntity.Owner.LastName;
            spaceDto.Goals = financialGoals.ToList();
            spaceDto.Members = members.ToList();
            spaceDto.RecentTransactions = transactions.ToList();

            return spaceDto;
        }

        public async Task<IEnumerable<FinancialSpaceDto>> GetFinancialSpacesUserIsMemberOrOwnerOf(int userId)
        {
            var spaces = await _financialSpaceRepository.GetSpacesUserIsMemberOrOwnerOf(userId);

            return spaces.Select(space => new FinancialSpaceDto
            {
                Id = space.Id,
                Name = space.Name,
                Description = space.Description,
                ImageName = space.ImageName,
                ImageData = space.ImageData,
                OwnerName = space.Owner?.FirstName ?? "Unknown",
                OwnerLastName = space.Owner?.LastName ?? "User",
            });
        }
    }
}