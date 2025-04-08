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
            _logger.LogInformation("Getting members for space ID {SpaceId}", spaceId);
            var members = await _financialSpaceMemberRepository.GetMembersBySpaceIdAsync(spaceId);
            _logger.LogInformation("Found {Count} members for space ID {SpaceId}", members.Count(), spaceId);

            var mappedMembers = new List<FinancialSpaceMemberDto>();

            foreach (var member in members)
            {
                var userResult = await _userService.GetUserByIdAsync(member.UserId);
                if (userResult.Success)
                {
                    var memberDto = new FinancialSpaceMemberDto
                    {
                        Id = userResult.Value.Id,
                        Email = userResult.Value.Email,
                        FirstName = userResult.Value.FirstName,
                        LastName = userResult.Value.LastName,
                        RegisteredAt = userResult.Value.RegisteredAt,
                        Role = userResult.Value.Role,
                        Categories = userResult.Value.Categories,
                        FinancialSpaceId = member.FinancialSpaceId,
                        MemberRole = member.Role
                    };

                    _logger.LogInformation(
                        "Created member DTO - Id: {Id}, Email: {Email}, Role: {Role}",
                        memberDto.Id,
                        memberDto.Email ?? "No Email",
                        memberDto.MemberRole);

                    mappedMembers.Add(memberDto);
                }
                else
                {
                    _logger.LogWarning("Could not find user info for member with UserId: {UserId}", member.UserId);
                }
            }

            _logger.LogInformation("Created {Count} member DTOs", mappedMembers.Count);
            return mappedMembers;
        }

        public async Task<IEnumerable<FinancialSpaceMemberDto>> GetBannedMembersBySpaceIdAsync(int spaceId)
        {
            _logger.LogInformation("Getting banned members for space ID {SpaceId}", spaceId);
            var bannedMembers = await _financialSpaceMemberRepository.GetBannedMembersBySpaceIdAsync(spaceId);
            _logger.LogInformation("Found {Count} banned members for space ID {SpaceId}", bannedMembers.Count(), spaceId);

            var mappedMembers = new List<FinancialSpaceMemberDto>();

            foreach (var member in bannedMembers)
            {
                var userResult = await _userService.GetUserByIdAsync(member.UserId);
                if (userResult.Success)
                {
                    var memberDto = new FinancialSpaceMemberDto
                    {
                        Id = userResult.Value.Id,
                        Email = userResult.Value.Email,
                        FirstName = userResult.Value.FirstName,
                        LastName = userResult.Value.LastName,
                        RegisteredAt = userResult.Value.RegisteredAt,
                        Role = userResult.Value.Role,
                        Categories = userResult.Value.Categories,
                        FinancialSpaceId = member.FinancialSpaceId,
                        MemberRole = member.Role
                    };

                    _logger.LogInformation(
                        "Created banned member DTO - Id: {Id}, Email: {Email}, Role: {Role}",
                        memberDto.Id,
                        memberDto.Email ?? "No Email",
                        memberDto.MemberRole);

                    mappedMembers.Add(memberDto);
                }
                else
                {
                    _logger.LogWarning("Could not find user info for banned member with UserId: {UserId}", member.UserId);
                }
            }

            _logger.LogInformation("Created {Count} banned member DTOs", mappedMembers.Count);
            return mappedMembers;
        }

        public async Task<string> BanMemberAsync(int spaceId, int memberId, int requestingUserId)
        {
            try
            {
                var members = await _financialSpaceMemberRepository.GetMembersBySpaceIdAsync(spaceId);
                var requestingMember = members.FirstOrDefault(m => m.UserId == requestingUserId);
                var memberToBan = members.FirstOrDefault(m => m.UserId == memberId);

                if (requestingMember == null || requestingMember.Role != "Owner")
                {
                    return "Only the owner can ban members.";
                }

                if (memberToBan == null)
                {
                    return "Member not found.";
                }

                if (memberToBan.Role == "Owner")
                {
                    return "Cannot ban the owner.";
                }

                if (memberToBan.Role == "Banned")
                {
                    return "Member is already banned.";
                }

                memberToBan.Role = "Banned";
                await _financialSpaceMemberRepository.UpdateAsync(memberToBan);

                return "Member successfully banned.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while banning member {MemberId} from space {SpaceId}", memberId, spaceId);
                return "An error occurred while banning the member.";
            }
        }

        public async Task<string> UnbanMemberAsync(int spaceId, int memberId, int requestingUserId)
        {
            try
            {
                var bannedMembers = await _financialSpaceMemberRepository.GetBannedMembersBySpaceIdAsync(spaceId);
                var members = await _financialSpaceMemberRepository.GetMembersBySpaceIdAsync(spaceId);
                var requestingMember = members.FirstOrDefault(m => m.UserId == requestingUserId);
                var memberToUnban = bannedMembers.FirstOrDefault(m => m.UserId == memberId);

                if (requestingMember == null || requestingMember.Role != "Owner")
                {
                    _logger.LogWarning("Only the owner can unban members.");
                    return "Only the owner can unban members.";
                }

                if (memberToUnban == null)
                {
                    _logger.LogWarning("Member not found.");
                    return "Member not found.";
                }

                if (memberToUnban.Role == "Owner")
                {
                    _logger.LogWarning("Cannot unban the owner since the owner can't be banned.");
                    return "Cannot unban the owner since the owner can't be banned.";
                }

                if (memberToUnban.Role != "Banned")
                {
                    _logger.LogWarning("Not banned members cannot be unbanned.");
                    return "Not banned members cannot be unbanned";
                }

                memberToUnban.Role = "Member";
                await _financialSpaceMemberRepository.UpdateAsync(memberToUnban);

                return "Member successfully unbanned.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while unbanning member {MemberId} from space {SpaceId}", memberId, spaceId);
                return "An error occurred while unbanning the member.";
            }
        }

        public async Task<string> DeleteMemberAsync(int spaceId, int memberId, int requestingUserId)
        {
            try
            {
                var members = await _financialSpaceMemberRepository.GetMembersBySpaceIdAsync(spaceId);
                var requestingMember = members.FirstOrDefault(m => m.UserId == requestingUserId);
                var memberToDelete = members.FirstOrDefault(m => m.UserId == memberId);

                if (requestingMember == null || requestingMember.Role != "Owner")
                {
                    return "Only the owner can delete members.";
                }

                if (memberToDelete == null)
                {
                    return "Member not found.";
                }

                if (memberToDelete.Role == "Owner")
                {
                    return "Cannot delete the owner.";
                }

                await _financialSpaceMemberRepository.DeleteAsync(memberToDelete);

                return "Member successfully deleted.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting member {MemberId} from space {SpaceId}", memberId, spaceId);
                return "An error occurred while deleting the member.";
            }
        }

        public async Task<FinancialSpaceMemberDto> CreateAsync(CreateFinancialSpaceMemberDto createDto)
        {
            try
            {
                _logger.LogInformation("Creating new member for user {UserId} in space {SpaceId}", createDto.UserId, createDto.FinancialSpaceId);
                var entity = _mapper.Map<FinancialSpaceMemberEntity>(createDto);
                var created = await _financialSpaceMemberRepository.CreateAsync(entity);
                _logger.LogInformation("Successfully created member for user {UserId} in space {SpaceId}", createDto.UserId, createDto.FinancialSpaceId);
                return _mapper.Map<FinancialSpaceMemberDto>(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating member for user {UserId} in space {SpaceId}", createDto.UserId, createDto.FinancialSpaceId);
                throw;
            }
        }

        public async Task InviteMember(string email, int spaceId)
        {
            _logger.LogInformation("Attempting to invite user with email {Email} to space ID {SpaceId}", email, spaceId);

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email cannot be empty", nameof(email));
            }

            var userResult = await _userService.GetUserByEmailAsync(email);
            if (!userResult.Success)
            {
                _logger.LogError("User with email {Email} not found", email);
                throw new InvalidOperationException($"User with email {email} not found");
            }

            var existingMembers = await _financialSpaceMemberRepository.GetMembersBySpaceIdAsync(spaceId);
            if (existingMembers.Any(m => m.UserId == userResult.Value.Id))
            {
                _logger.LogWarning("User with email {Email} is already a member of space {SpaceId}", email, spaceId);
                throw new InvalidOperationException($"User with email {email} is already a member of this space");
            }

            var newMember = new CreateFinancialSpaceMemberDto
            {
                UserId = userResult.Value.Id,
                FinancialSpaceId = spaceId,
                Role = "Member"
            };

            await CreateAsync(newMember);
            _logger.LogInformation("User with email {Email} successfully invited to space {SpaceId}", email, spaceId);
        }
    }
}
