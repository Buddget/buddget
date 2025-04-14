using AutoMapper;
using Buddget.BLL.DTOs;
using Buddget.BLL.Services.Interfaces;
using Buddget.DAL.Repositories.Interfaces;
using Buddget.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Buddget.BLL.Services.Implementations
{
    public class FinancialSpaceService : IFinancialSpaceService
    {
        private readonly IFinancialSpaceRepository _financialSpaceRepository;
        private readonly IFinancialSpaceMemberService _financialSpaceMemberService;
        private readonly IFinancialGoalSpaceService _financialGoalSpaceService;
        private readonly ITransactionService _transactionService;
        private readonly IMapper _mapper;
        private readonly ILogger<FinancialSpaceService> _logger;

        public FinancialSpaceService(
            IFinancialSpaceRepository financialSpaceRepository,
            IFinancialSpaceMemberService financialSpaceMemberService,
            IFinancialGoalSpaceService financialGoalSpaceService,
            ITransactionService transactionService,
            IMapper mapper,
            ILogger<FinancialSpaceService> logger)
        {
            _financialSpaceRepository = financialSpaceRepository;
            _financialSpaceMemberService = financialSpaceMemberService;
            _financialGoalSpaceService = financialGoalSpaceService;
            _transactionService = transactionService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<FinancialSpaceDto> GetFinancialSpaceByIdAsync(int spaceId)
        {
            _logger.LogInformation("Getting financial space with ID {SpaceId}", spaceId);

            // Get financial space
            var spaceEntity = await _financialSpaceRepository.GetFinancialSpaceAsync(spaceId);
            if (spaceEntity == null)
            {
                _logger.LogWarning("Financial space with ID {SpaceId} not found", spaceId);
                return null;
            }

            _logger.LogInformation("Found financial space - Name: {Name}, OwnerId: {OwnerId}", 
                spaceEntity.Name, spaceEntity.OwnerId);

            // Get members of the space
            var members = await _financialSpaceMemberService.GetMembersBySpaceIdAsync(spaceId);
            _logger.LogInformation("Retrieved {Count} members for space {SpaceId}", members.Count(), spaceId);

            // Get banned members of the space
            var bannedMembers = await _financialSpaceMemberService.GetBannedMembersBySpaceIdAsync(spaceId);
            _logger.LogInformation("Retrieved {Count} banned members for space {SpaceId}", bannedMembers.Count(), spaceId);

            // Get financial goals of the space
            var financialGoals = await _financialGoalSpaceService.GetFinancialGoalsBySpaceIdAsync(spaceId);
            _logger.LogInformation("Retrieved {Count} goals for space {SpaceId}", financialGoals.Count(), spaceId);

            // Get transactions of the space
            var transactions = await _transactionService.GetTransactionsBySpaceIdAsync(spaceId);
            _logger.LogInformation("Retrieved {Count} transactions for space {SpaceId}", transactions.Count(), spaceId);

            // Map to DTO
            var spaceDto = _mapper.Map<FinancialSpaceDto>(spaceEntity);
            spaceDto.OwnerName = spaceEntity.Owner?.FirstName + " " + spaceEntity.Owner?.LastName;
            spaceDto.Goals = financialGoals.ToList();
            spaceDto.Members = members.ToList();
            spaceDto.BannedMembers = bannedMembers.ToList();
            spaceDto.RecentTransactions = transactions.ToList();

            _logger.LogInformation("Mapped financial space to DTO - Members count: {MembersCount}, Banned members count: {BannedCount}", 
                spaceDto.Members.Count, spaceDto.BannedMembers.Count);

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

        public async Task<string> DeleteFinancialSpaceAsync(int userId, int id)
        {
            var space = await _financialSpaceRepository.GetFinancialSpaceAsync(id);
            if (space == null)
            {
                _logger.LogWarning("Financial space with ID {SpaceId} was not found.", id);
                return "Financial space not found.";
            }

            if (space.OwnerId != userId)
            {
                _logger.LogWarning("User with ID {UserId} is not the owner of financial space with ID {SpaceId}.", userId, id);
                return "You are not the owner of this financial space.";
            }

            try
            {
                await _financialSpaceRepository.DeleteAsync(space);
                _logger.LogInformation("Successfully deleted financial space with ID {SpaceId}.", id);
                return "Financial space deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to delete financial space with ID {SpaceId}.", id);
                return "An error occurred while deleting the financial space.";
            }
        }

        public async Task<(FinancialSpaceDto? createdSpace, string resultMessage)> CreateFinancialSpaceAsync(FinancialSpaceDto financialSpaceDto)
        {
            if (financialSpaceDto == null)
            {
                _logger.LogWarning("Financial space data is required.");
                return (null, "Financial space data is required.");
            }

            if (string.IsNullOrWhiteSpace(financialSpaceDto.Name))
            {
                _logger.LogWarning("Name is required.");
                return (null, "Name is required.");
            }

            if (financialSpaceDto.Description?.Length > 1000)
            {
                _logger.LogWarning("Description cannot exceed 1000 characters.");
                return (null, "Description cannot exceed 1000 characters.");
            }

            if ((financialSpaceDto.ImageData == null) != (financialSpaceDto.ImageName == null))
            {
                _logger.LogWarning("When providing ImageData, ImageName is required and vice versa.");
                return (null, "When providing ImageData, ImageName is required and vice versa.");
            }

            var financialSpace = await GetFinancialSpaceByIdAsync(financialSpaceDto.Id);
            if (financialSpace != null)
            {
                _logger.LogWarning($"Financial space with ID={financialSpace.Id} already exists.");
                return (null, $"Financial space with ID={financialSpace.Id} already exists.");
            }

            var spaceEntity = _mapper.Map<FinancialSpaceEntity>(financialSpaceDto);
            spaceEntity.CreatedAt = DateTime.UtcNow;

            await _financialSpaceRepository.AddAsync(spaceEntity);

            _logger.LogInformation("Financial space with ID {SpaceId} created successfully.", spaceEntity.Id);

            var createdDto = _mapper.Map<FinancialSpaceDto>(spaceEntity);
            return (createdDto, "Financial space created successfully.");
        }
    }
}