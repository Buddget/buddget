﻿using Moq;
using Buddget.BLL.Services.Implementation;
using Buddget.DAL.Repositories.Interfaces;
using Buddget.BLL.Services.Interfaces;
using AutoMapper;
using Buddget.DAL.Entities;
using Microsoft.Extensions.Logging;
using Buddget.BLL.DTOs;
using Buddget.BLL.Utilities;

namespace Buddget.Tests.Services.Implementation
{
    public class FinancialSpaceMemberServiceTests
    {
        private readonly Mock<IFinancialSpaceMemberRepository> _mockFinancialSpaceMemberRepository;
        private readonly Mock<IUserService> _mockUserService;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<FinancialSpaceMemberService>> _mockLogger;
        private readonly FinancialSpaceMemberService _service;

        public FinancialSpaceMemberServiceTests()
        {
            // Initialize the mock repository
            _mockFinancialSpaceMemberRepository = new Mock<IFinancialSpaceMemberRepository>();
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<FinancialSpaceMemberService>>();
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile(new FinancialSpaceMemberProfile())).CreateMapper();

            // Create the service with mocked dependencies
            _service = new FinancialSpaceMemberService(
                _mockFinancialSpaceMemberRepository.Object,
                _mockUserService.Object,
                _mapper,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task GetMembersBySpaceIdAsync_ReturnsMappedMembers()
        {
            // Arrange
            var spaceId = 1;
            var members = new List<FinancialSpaceMemberEntity>
            {
                new FinancialSpaceMemberEntity { Id = 1, UserId = 1, Role = "Member" },
                new FinancialSpaceMemberEntity { Id = 2, UserId = 2, Role = "Member" }
            };

            _mockFinancialSpaceMemberRepository
                .Setup(repo => repo.GetMembersBySpaceIdAsync(spaceId))
                .ReturnsAsync(members);

            _mockUserService
                .Setup(service => service.GetUserByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => Result<UserDto>.SuccessResult(new UserDto 
                { 
                    Id = id, 
                    Email = $"user{id}@example.com",
                    FirstName = $"User{id}",
                    LastName = "Test",
                    RegisteredAt = DateTime.Now
                }));

            // Act
            var result = await _service.GetMembersBySpaceIdAsync(spaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("user1@example.com", result.First().Email);
            Assert.Equal("Member", result.First().MemberRole);
        }

        [Fact]
        public async Task GetBannedMembersBySpaceIdAsync_ReturnsMappedBannedMembers()
        {
            // Arrange
            var spaceId = 1;
            var bannedMembers = new List<FinancialSpaceMemberEntity>
            {
                new FinancialSpaceMemberEntity { Id = 1, UserId = 1, Role = "Banned" }
            };

            _mockFinancialSpaceMemberRepository
                .Setup(repo => repo.GetBannedMembersBySpaceIdAsync(spaceId))
                .ReturnsAsync(bannedMembers);

            _mockUserService
                .Setup(service => service.GetUserByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => Result<UserDto>.SuccessResult(new UserDto 
                { 
                    Id = id, 
                    Email = $"user{id}@example.com",
                    FirstName = $"User{id}",
                    LastName = "Test",
                    RegisteredAt = DateTime.Now
                }));

            // Act
            var result = await _service.GetBannedMembersBySpaceIdAsync(spaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("user1@example.com", result.First().Email);
            Assert.Equal("Banned", result.First().MemberRole);
        }

        [Fact]
        public async Task BanMemberAsync_Fails_WhenMemberAlreadyBanned()
        {
            // Arrange
            var spaceId = 1;
            var memberId = 2;
            var ownerId = 1;

            var members = new List<FinancialSpaceMemberEntity>
            {
                new FinancialSpaceMemberEntity { Id = 1, UserId = ownerId, Role = "Owner" },
                new FinancialSpaceMemberEntity { Id = 2, UserId = memberId, Role = "Banned" }
            };

            _mockFinancialSpaceMemberRepository
                .Setup(repo => repo.GetMembersBySpaceIdAsync(spaceId))
                .ReturnsAsync(members);

            // Act
            var result = await _service.BanMemberAsync(spaceId, memberId, ownerId);

            // Assert
            Assert.Contains("Member is already banned", result);
        }

        [Fact]
        public async Task BanMemberAsync_HandlesUpdateException()
        {
            // Arrange
            var spaceId = 1;
            var memberId = 2;
            var ownerId = 1;

            var members = new List<FinancialSpaceMemberEntity>
            {
                new FinancialSpaceMemberEntity { Id = 1, UserId = ownerId, Role = "Owner" },
                new FinancialSpaceMemberEntity { Id = 2, UserId = memberId, Role = "Member" }
            };

            _mockFinancialSpaceMemberRepository
                .Setup(repo => repo.GetMembersBySpaceIdAsync(spaceId))
                .ReturnsAsync(members);

            _mockFinancialSpaceMemberRepository
                .Setup(repo => repo.UpdateAsync(It.IsAny<FinancialSpaceMemberEntity>()))
                .ThrowsAsync(new Exception("Update failed"));

            // Act
            var result = await _service.BanMemberAsync(spaceId, memberId, ownerId);

            // Assert
            Assert.Contains("An error occurred while banning the member", result);
        }

        [Fact]
        public async Task InviteMember_SuccessfullyInvitesNewMember()
        {
            // Arrange
            var email = "test@example.com";
            var spaceId = 1;
            var userId = 1;

            var userDto = new UserDto
            {
                Id = userId,
                Email = email,
                FirstName = "Test",
                LastName = "User",
                RegisteredAt = DateTime.Now
            };

            _mockUserService
                .Setup(service => service.GetUserByEmailAsync(email))
                .ReturnsAsync(Result<UserDto>.SuccessResult(userDto));

            _mockFinancialSpaceMemberRepository
                .Setup(repo => repo.GetMembersBySpaceIdAsync(spaceId))
                .ReturnsAsync(new List<FinancialSpaceMemberEntity>());

            _mockFinancialSpaceMemberRepository
                .Setup(repo => repo.CreateAsync(It.IsAny<FinancialSpaceMemberEntity>()))
                .ReturnsAsync((FinancialSpaceMemberEntity entity) => entity);

            // Act
            await _service.InviteMember(email, spaceId);

            // Assert
            _mockFinancialSpaceMemberRepository.Verify(
                repo => repo.CreateAsync(It.Is<FinancialSpaceMemberEntity>(entity =>
                    entity.UserId == userId &&
                    entity.FinancialSpaceId == spaceId &&
                    entity.Role == "Member")),
                Times.Once);
        }

        [Fact]
        public async Task InviteMember_ThrowsException_WhenEmailIsEmpty()
        {
            // Arrange
            var email = "";
            var spaceId = 1;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.InviteMember(email, spaceId));
        }

        [Fact]
        public async Task InviteMember_ThrowsException_WhenUserNotFound()
        {
            // Arrange
            var email = "nonexistent@example.com";
            var spaceId = 1;

            _mockUserService
                .Setup(service => service.GetUserByEmailAsync(email))
                .ReturnsAsync(Result<UserDto>.FailureResult("User not found"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _service.InviteMember(email, spaceId));
            Assert.Contains("not found", exception.Message);
        }

        [Fact]
        public async Task InviteMember_ThrowsException_WhenUserAlreadyMember()
        {
            // Arrange
            var email = "existing@example.com";
            var spaceId = 1;
            var userId = 1;

            var userDto = new UserDto
            {
                Id = userId,
                Email = email,
                FirstName = "Test",
                LastName = "User",
                RegisteredAt = DateTime.Now
            };

            _mockUserService
                .Setup(service => service.GetUserByEmailAsync(email))
                .ReturnsAsync(Result<UserDto>.SuccessResult(userDto));

            _mockFinancialSpaceMemberRepository
                .Setup(repo => repo.GetMembersBySpaceIdAsync(spaceId))
                .ReturnsAsync(new List<FinancialSpaceMemberEntity> 
                { 
                    new FinancialSpaceMemberEntity 
                    { 
                        UserId = userId,
                        FinancialSpaceId = spaceId,
                        Role = "Member"
                    } 
                });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _service.InviteMember(email, spaceId));
            Assert.Contains("already a member", exception.Message);
        }
    }
}
