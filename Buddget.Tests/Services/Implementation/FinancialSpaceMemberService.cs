using Moq;
using Buddget.BLL.Services.Implementation;
using Buddget.DAL.Repositories.Interfaces;
using AutoMapper;
using Buddget.DAL.Entities;

namespace Buddget.Tests.Services.Implementation
{
    public class FinancialSpaceMemberServiceTests
    {
        private readonly Mock<IFinancialSpaceMemberRepository> _mockFinancialSpaceMemberRepository;
        private readonly IMapper _mapper;
        private readonly FinancialSpaceMemberService _service;

        public FinancialSpaceMemberServiceTests()
        {
            // Arrange AutoMapper configuration
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                // Assuming you have a profile or mapping configuration for FinancialSpaceMemberDto
                cfg.AddProfile(new FinancialSpaceMemberProfile()); // Adjust this based on your profile setup
            });
            _mapper = mapperConfig.CreateMapper();

            // Initialize the mock repository
            _mockFinancialSpaceMemberRepository = new Mock<IFinancialSpaceMemberRepository>();

            // Create the service with mocked dependencies
            _service = new FinancialSpaceMemberService(_mockFinancialSpaceMemberRepository.Object, _mapper);
        }

        [Fact]
        public async Task GetMembersBySpaceIdAsync_ReturnsMappedMembers()
        {
            // Arrange
            var spaceId = 1;
            var financialSpaceMemberEntities = new List<FinancialSpaceMemberEntity>
            {
                new FinancialSpaceMemberEntity
                {
                    Id = 1,
                    FinancialSpaceId = spaceId,
                    UserId = 1,
                    Role = "Admin",
                    FinancialSpace = new FinancialSpaceEntity { Id = spaceId, Name = "Financial Space 1" },
                    User = new UserEntity { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Role = "user", RegisteredAt = DateTime.Now }
                },
                new FinancialSpaceMemberEntity
                {
                    Id = 2,
                    FinancialSpaceId = spaceId,
                    UserId = 2,
                    Role = "Member",
                    FinancialSpace = new FinancialSpaceEntity { Id = spaceId, Name = "Financial Space 1" },
                    User = new UserEntity { Id = 2, FirstName = "Jane", LastName = "Doe", Email = "jane.doe@example.com", Role = "user", RegisteredAt = DateTime.Now }
                }
            };

            // Mock the repository method
            _mockFinancialSpaceMemberRepository
                .Setup(repo => repo.GetMembersBySpaceIdAsync(spaceId))
                .ReturnsAsync(financialSpaceMemberEntities);

            // Act
            var result = await _service.GetMembersBySpaceIdAsync(spaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("John", result.First().FirstName);
            Assert.Equal("Admin", result.First().Role);
            _mockFinancialSpaceMemberRepository.Verify(repo => repo.GetMembersBySpaceIdAsync(spaceId), Times.Once);
        }

        [Fact]
        public async Task GetMembersBySpaceIdAsync_ReturnsEmptyList_WhenNoMembers()
        {
            // Arrange
            var spaceId = 1;
            var financialSpaceMemberEntities = new List<FinancialSpaceMemberEntity>();

            // Mock the repository method
            _mockFinancialSpaceMemberRepository
                .Setup(repo => repo.GetMembersBySpaceIdAsync(spaceId))
                .ReturnsAsync(financialSpaceMemberEntities);

            // Act
            var result = await _service.GetMembersBySpaceIdAsync(spaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockFinancialSpaceMemberRepository.Verify(repo => repo.GetMembersBySpaceIdAsync(spaceId), Times.Once);
        }

        [Fact]
        public async Task GetBannedMembersBySpaceIdAsync_ReturnsMappedBannedMembers()
        {
            // Arrange
            var spaceId = 1;
            var bannedMembers = new List<FinancialSpaceMemberEntity>
    {
        new FinancialSpaceMemberEntity
        {
            Id = 3,
            FinancialSpaceId = spaceId,
            UserId = 3,
            Role = "Banned",
            FinancialSpace = new FinancialSpaceEntity { Id = spaceId, Name = "Financial Space 1" },
            User = new UserEntity { Id = 3, FirstName = "Alice", LastName = "Smith", Email = "alice.smith@example.com", Role = "user", RegisteredAt = DateTime.Now }
        }
    };

            // Mock the repository method
            _mockFinancialSpaceMemberRepository
                .Setup(repo => repo.GetBannedMembersBySpaceIdAsync(spaceId))
                .ReturnsAsync(bannedMembers);

            // Act
            var result = await _service.GetBannedMembersBySpaceIdAsync(spaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Alice", result.First().FirstName);
            Assert.Equal("Banned", result.First().MemberRole);
            _mockFinancialSpaceMemberRepository.Verify(repo => repo.GetBannedMembersBySpaceIdAsync(spaceId), Times.Once);
        }

        [Fact]
        public async Task GetBannedMembersBySpaceIdAsync_ReturnsEmptyList_WhenNoBannedMembers()
        {
            // Arrange
            var spaceId = 1;
            var bannedMembers = new List<FinancialSpaceMemberEntity>();

            // Mock the repository method
            _mockFinancialSpaceMemberRepository
                .Setup(repo => repo.GetBannedMembersBySpaceIdAsync(spaceId))
                .ReturnsAsync(bannedMembers);

            // Act
            var result = await _service.GetBannedMembersBySpaceIdAsync(spaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockFinancialSpaceMemberRepository.Verify(repo => repo.GetBannedMembersBySpaceIdAsync(spaceId), Times.Once);
        }

        [Fact]
        public async Task BanMemberAsync_Success_WhenRequestingUserIsOwner()
        {
            // Arrange
            var spaceId = 1;
            var memberId = 2;
            var ownerId = 1;

            var members = new List<FinancialSpaceMemberEntity>
    {
        new FinancialSpaceMemberEntity
        {
            Id = 1,
            FinancialSpaceId = spaceId,
            UserId = ownerId,
            Role = "Owner",
            User = new UserEntity { Id = ownerId, FirstName = "John", LastName = "Owner", Email = "owner@example.com" }
        },
        new FinancialSpaceMemberEntity
        {
            Id = 2,
            FinancialSpaceId = spaceId,
            UserId = memberId,
            Role = "Member",
            User = new UserEntity { Id = memberId, FirstName = "Jane", LastName = "Member", Email = "member@example.com" }
        }
    };

            var bannedMembers = new List<FinancialSpaceMemberEntity>();

            _mockFinancialSpaceMemberRepository
                .Setup(repo => repo.GetMembersBySpaceIdAsync(spaceId))
                .ReturnsAsync(members);

            _mockFinancialSpaceMemberRepository
                .Setup(repo => repo.GetBannedMembersBySpaceIdAsync(spaceId))
                .ReturnsAsync(bannedMembers);

            // Act
            var result = await _service.BanMemberAsync(spaceId, memberId, ownerId);

            // Assert
            Assert.Contains("successfully banned", result.ToLower());
            _mockFinancialSpaceMemberRepository.Verify(repo => repo.UpdateAsync(It.Is<FinancialSpaceMemberEntity>(m =>
                m.UserId == memberId && m.Role == "Banned")), Times.Once);
        }

        [Fact]
        public async Task BanMemberAsync_Fails_WhenRequestingUserIsNotOwner()
        {
            // Arrange
            var spaceId = 1;
            var memberId = 2;
            var ownerId = 1;
            var requestingUserId = 3; // Not the owner

            var members = new List<FinancialSpaceMemberEntity>
    {
        new FinancialSpaceMemberEntity
        {
            Id = 1,
            FinancialSpaceId = spaceId,
            UserId = ownerId,
            Role = "Owner",
            User = new UserEntity { Id = ownerId, FirstName = "John", LastName = "Owner", Email = "owner@example.com" }
        },
        new FinancialSpaceMemberEntity
        {
            Id = 2,
            FinancialSpaceId = spaceId,
            UserId = memberId,
            Role = "Member",
            User = new UserEntity { Id = memberId, FirstName = "Jane", LastName = "Member", Email = "member@example.com" }
        }
    };

            _mockFinancialSpaceMemberRepository
                .Setup(repo => repo.GetMembersBySpaceIdAsync(spaceId))
                .ReturnsAsync(members);

            // Act
            var result = await _service.BanMemberAsync(spaceId, memberId, requestingUserId);

            // Assert
            Assert.Contains("only the owner", result.ToLower());
            _mockFinancialSpaceMemberRepository.Verify(repo => repo.UpdateAsync(It.IsAny<FinancialSpaceMemberEntity>()), Times.Never);
        }

        [Fact]
        public async Task BanMemberAsync_Fails_WhenMemberNotFound()
        {
            // Arrange
            var spaceId = 1;
            var memberId = 99; // Non-existent member
            var ownerId = 1;

            var members = new List<FinancialSpaceMemberEntity>
    {
        new FinancialSpaceMemberEntity
        {
            Id = 1,
            FinancialSpaceId = spaceId,
            UserId = ownerId,
            Role = "Owner",
            User = new UserEntity { Id = ownerId, FirstName = "John", LastName = "Owner", Email = "owner@example.com" }
        }
    };

            _mockFinancialSpaceMemberRepository
                .Setup(repo => repo.GetMembersBySpaceIdAsync(spaceId))
                .ReturnsAsync(members);

            // Act
            var result = await _service.BanMemberAsync(spaceId, memberId, ownerId);

            // Assert
            Assert.Contains("member not found", result.ToLower());
            _mockFinancialSpaceMemberRepository.Verify(repo => repo.UpdateAsync(It.IsAny<FinancialSpaceMemberEntity>()), Times.Never);
        }

        [Fact]
        public async Task BanMemberAsync_Fails_WhenTryingToBanOwner()
        {
            // Arrange
            var spaceId = 1;
            var ownerId = 1;

            var members = new List<FinancialSpaceMemberEntity>
    {
        new FinancialSpaceMemberEntity
        {
            Id = 1,
            FinancialSpaceId = spaceId,
            UserId = ownerId,
            Role = "Owner",
            User = new UserEntity { Id = ownerId, FirstName = "John", LastName = "Owner", Email = "owner@example.com" }
        }
    };

            _mockFinancialSpaceMemberRepository
                .Setup(repo => repo.GetMembersBySpaceIdAsync(spaceId))
                .ReturnsAsync(members);

            // Act
            var result = await _service.BanMemberAsync(spaceId, ownerId, ownerId);

            // Assert
            Assert.Contains("cannot ban the owner", result.ToLower());
            _mockFinancialSpaceMemberRepository.Verify(repo => repo.UpdateAsync(It.IsAny<FinancialSpaceMemberEntity>()), Times.Never);
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
        new FinancialSpaceMemberEntity
        {
            Id = 1,
            FinancialSpaceId = spaceId,
            UserId = ownerId,
            Role = "Owner",
            User = new UserEntity { Id = ownerId, FirstName = "John", LastName = "Owner", Email = "owner@example.com" }
        },
        new FinancialSpaceMemberEntity
        {
            Id = 2,
            FinancialSpaceId = spaceId,
            UserId = memberId,
            Role = "Member",
            User = new UserEntity { Id = memberId, FirstName = "Jane", LastName = "Member", Email = "member@example.com" }
        }
    };

            var bannedMembers = new List<FinancialSpaceMemberEntity>
    {
        new FinancialSpaceMemberEntity
        {
            Id = 2,
            FinancialSpaceId = spaceId,
            UserId = memberId,
            Role = "Banned",
            User = new UserEntity { Id = memberId, FirstName = "Jane", LastName = "Member", Email = "member@example.com" }
        }
    };

            _mockFinancialSpaceMemberRepository
                .Setup(repo => repo.GetMembersBySpaceIdAsync(spaceId))
                .ReturnsAsync(members);

            _mockFinancialSpaceMemberRepository
                .Setup(repo => repo.GetBannedMembersBySpaceIdAsync(spaceId))
                .ReturnsAsync(bannedMembers);

            // Act
            var result = await _service.BanMemberAsync(spaceId, memberId, ownerId);

            // Assert
            Assert.Contains("already banned", result.ToLower());
            _mockFinancialSpaceMemberRepository.Verify(repo => repo.UpdateAsync(It.IsAny<FinancialSpaceMemberEntity>()), Times.Never);
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
        new FinancialSpaceMemberEntity
        {
            Id = 1,
            FinancialSpaceId = spaceId,
            UserId = ownerId,
            Role = "Owner",
            User = new UserEntity { Id = ownerId, FirstName = "John", LastName = "Owner", Email = "owner@example.com" }
        },
        new FinancialSpaceMemberEntity
        {
            Id = 2,
            FinancialSpaceId = spaceId,
            UserId = memberId,
            Role = "Member",
            User = new UserEntity { Id = memberId, FirstName = "Jane", LastName = "Member", Email = "member@example.com" }
        }
    };

            var bannedMembers = new List<FinancialSpaceMemberEntity>();

            _mockFinancialSpaceMemberRepository
                .Setup(repo => repo.GetMembersBySpaceIdAsync(spaceId))
                .ReturnsAsync(members);

            _mockFinancialSpaceMemberRepository
                .Setup(repo => repo.GetBannedMembersBySpaceIdAsync(spaceId))
                .ReturnsAsync(bannedMembers);

            _mockFinancialSpaceMemberRepository
                .Setup(repo => repo.UpdateAsync(It.IsAny<FinancialSpaceMemberEntity>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _service.BanMemberAsync(spaceId, memberId, ownerId);

            // Assert
            Assert.Contains("failed to ban member", result.ToLower());
        }

        [Fact]
        public async Task BanMemberAsync_HandlesGeneralException()
        {
            // Arrange
            var spaceId = 1;
            var memberId = 2;
            var ownerId = 1;

            _mockFinancialSpaceMemberRepository
                .Setup(repo => repo.GetMembersBySpaceIdAsync(spaceId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _service.BanMemberAsync(spaceId, memberId, ownerId);

            // Assert
            Assert.Contains("an error occurred", result.ToLower());
        }
    }
}
