using AutoMapper;
using Buddget.BLL.DTOs;
using Buddget.BLL.Services.Interfaces;
using Buddget.DAL.Entities;
using Buddget.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
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
            // Get financial space
            var spaceEntity = await _financialSpaceRepository.GetFinancialSpaceAsync(spaceId);
            if (spaceEntity == null)
            {
                return null;
            }

            // Get members of the space
            var members = await _financialSpaceMemberService.GetMembersBySpaceIdAsync(spaceId);

            // Get banned members of the space
            var bannedMembers = await _financialSpaceMemberService.GetBannedMembersBySpaceIdAsync(spaceId);

            // Get financial goals of the space
            var financialGoals = await _financialGoalSpaceService.GetFinancialGoalsBySpaceIdAsync(spaceId);

            // Get transactions of the space
            var transactions = await _transactionService.GetTransactionsBySpaceIdAsync(spaceId);

            // Map to DTO
            var spaceDto = _mapper.Map<FinancialSpaceDto>(spaceEntity);
            spaceDto.OwnerName = spaceEntity.Owner?.FirstName + " " + spaceEntity.Owner?.LastName;
            spaceDto.Goals = financialGoals.ToList();
            spaceDto.Members = members.ToList();
            spaceDto.BannedMembers = bannedMembers.ToList();
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

        public async Task<string> CreateFinancialSpaceAsync(FinancialSpaceDto financialSpaceDto)
        {
            if (financialSpaceDto == null)
            {
                _logger.LogWarning("Financial space data is required.");
                return "Financial space data is required.";
            }

            if (financialSpaceDto.Name == null)
            {
                _logger.LogWarning("Name is required.");
                return "Name is required.";
            }

            if (financialSpaceDto.Description != null && financialSpaceDto.Description.Length > 1000)
            {
                _logger.LogWarning("Description cannot exceed 1000 characters.");
                return "Description cannot exceed 1000 characters.";
            }

            if ((financialSpaceDto.ImageData == null) != (financialSpaceDto.ImageData == null))
            {
                _logger.LogWarning("When providing ImageData, ImageName is required and vica verca.");
                return "When providing ImageData, ImageName is required and vica verca.";
            }

            var financialSpace = await GetFinancialSpaceByIdAsync(financialSpaceDto.Id);
            if (financialSpace != null)
            {
                _logger.LogWarning($"Financial space with ID={financialSpace.Id} already exists.");
                return $"Financial space with ID={financialSpace.Id} already exists.";
            }

            var spaceEntity = _mapper.Map<FinancialSpaceEntity>(financialSpaceDto);

            spaceEntity.CreatedAt = DateTime.UtcNow;

            await _financialSpaceRepository.AddAsync(spaceEntity);

            _logger.LogInformation("Financial space with ID {SpaceId} created successfully.", spaceEntity.Id);
            return "Financial space created successfully.";
        }
    }
}