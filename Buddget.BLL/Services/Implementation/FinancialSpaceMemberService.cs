using AutoMapper;
using Buddget.BLL.DTOs;
using Buddget.BLL.Services.Interfaces;
using Buddget.DAL.Entities;
using Buddget.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Buddget.BLL.Services.Implementation
{
    public class FinancialSpaceMemberService : IFinancialSpaceMemberService
    {
        private readonly IFinancialSpaceMemberRepository _financialSpaceMemberRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly Random _random = new Random();
        private readonly ILogger<FinancialSpaceMemberService> _logger;

        public FinancialSpaceMemberService(
            IFinancialSpaceMemberRepository financialSpaceMemberRepository,
            IUserService userService,
            IMapper mapper,
            ILogger<FinancialSpaceMemberService> logger)
        {
            _financialSpaceMemberRepository = financialSpaceMemberRepository;
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<FinancialSpaceMemberDto>> GetMembersBySpaceIdAsync(int spaceId)
        {
            var members = await _financialSpaceMemberRepository.GetMembersBySpaceIdAsync(spaceId);
            return _mapper.Map<IEnumerable<FinancialSpaceMemberDto>>(members);
        }

        public async Task<CreateFinancialSpaceMemberDto> CreateAsync(CreateFinancialSpaceMemberDto createDto)
        {
            FinancialSpaceMemberEntity entity = _mapper.Map<FinancialSpaceMemberEntity>(createDto);
            var created = await _financialSpaceMemberRepository.CreateAsync(entity);
            return _mapper.Map<CreateFinancialSpaceMemberDto>(created);
        }

        public async Task InviteMember(string email, int spaceId)
        {
            _logger.LogInformation("Attempting to invite user with email {Email} to space ID {SpaceId}.", email, spaceId);
            try
            {
                var user = await _userService.GetByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogError("User with email {Email} does not exist.", email);
                    throw new Exception($"User with email {email} does not exist.");
                }

                var spaceMember = await _financialSpaceMemberRepository.GetMembershipAsync(user.Id, spaceId);
                if (spaceMember != null)
                {
                    _logger.LogError("User with email {Email} is already a member of space ID {SpaceId}.", email, spaceId);
                    throw new Exception($"User with email {email} is already a member of space ID {spaceId}.");
                }

                var createFinancialSpaceMemberDto = new CreateFinancialSpaceMemberDto
                {
                    UserId = user.Id,
                    FinancialSpaceId = spaceId,
                    Role = "User",
                };

                await CreateAsync(createFinancialSpaceMemberDto);
                _logger.LogInformation("User with email {Email} successfully invited to space ID {SpaceId}.", email, spaceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while inviting user {Email} to space ID {SpaceId}.", email, spaceId);
                throw new Exception($"Failed to invite user {email} to space {spaceId}.", ex);
            }
        }

    }
}
