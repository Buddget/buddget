using Moq;
using Buddget.BLL.Services.Implementation;
using Buddget.DAL.Repositories.Interfaces;
using AutoMapper;
using Buddget.DAL.Entities;
using Buddget.BLL.DTOs;

namespace Buddget.Tests.Services
{
    public class FinancialSpaceMemberServiceTests
    {
        private readonly Mock<IFinancialSpaceMemberRepository> _mockFinancialSpaceMemberRepository;
        private readonly Mock<IUserRepository> _mockUserServiceRepository;
        private readonly IMapper _mapper;
        private readonly FinancialSpaceMemberService _service;

        private readonly UserService _mockUserService;

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

            _mockUserServiceRepository = new Mock<IUserRepository>();

            // Create the service with mocked dependencies
            _service = new FinancialSpaceMemberService(_mockFinancialSpaceMemberRepository.Object, _mapper);

            _mockUserService = new UserService(_mockUserServiceRepository.Object, _mapper);
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
        public async Task CreateAsync_CreatesMemberAndReturnsDto()
        {
            // Arrange
            var createDto = new BLL.DTOs.CreateFinancialSpaceMemberDto
            {
                UserId = 1,
                FinancialSpaceId = 1,
                Role = "Member"
            };

            var entity = new FinancialSpaceMemberEntity
            {
                Id = 1,
                UserId = createDto.UserId,
                FinancialSpaceId = createDto.FinancialSpaceId,
                Role = createDto.Role
            };

            _mockFinancialSpaceMemberRepository
                .Setup(repo => repo.CreateAsync(It.IsAny<FinancialSpaceMemberEntity>()))
                .ReturnsAsync(entity);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createDto.UserId, result.UserId);
            Assert.Equal(createDto.FinancialSpaceId, result.FinancialSpaceId);
            Assert.Equal(createDto.Role, result.Role);

            _mockFinancialSpaceMemberRepository.Verify(repo => repo.CreateAsync(It.IsAny<FinancialSpaceMemberEntity>()), Times.Once);
        }

    }
}
