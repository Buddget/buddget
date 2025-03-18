using AutoMapper;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Buddget.BLL.DTOs;
using Buddget.BLL.Services.Implementations;
using Buddget.DAL.Entities;
using Buddget.DAL.Repositories.Interfaces;
using Xunit;
using Buddget.BLL.Mappers;
using Buddget.BLL.Services.Interfaces;
using System.Linq;

namespace Buddget.Tests.Services.Implementation
{
    public class FinancialSpaceServiceTests
    {
        private readonly Mock<IFinancialSpaceRepository> _mockFinancialSpaceRepository;
        private readonly Mock<IFinancialSpaceMemberService> _mockFinancialSpaceMemberService;
        private readonly Mock<IFinancialGoalSpaceService> _mockFinancialGoalSpaceService;
        private readonly Mock<ITransactionService> _mockTransactionService;
        private readonly IMapper _mapper;
        private readonly FinancialSpaceService _service;

        public FinancialSpaceServiceTests()
        {
            // Arrange AutoMapper configuration
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new FinancialSpaceProfile()); // Using the profile you've set up
            });
            _mapper = mapperConfig.CreateMapper();

            // Initialize the mock repositories and services
            _mockFinancialSpaceRepository = new Mock<IFinancialSpaceRepository>();
            _mockFinancialSpaceMemberService = new Mock<IFinancialSpaceMemberService>();
            _mockFinancialGoalSpaceService = new Mock<IFinancialGoalSpaceService>();
            _mockTransactionService = new Mock<ITransactionService>();

            // Create the service with mocked dependencies
            _service = new FinancialSpaceService(
                _mockFinancialSpaceRepository.Object,
                _mockFinancialSpaceMemberService.Object,
                _mockFinancialGoalSpaceService.Object,
                _mockTransactionService.Object,
                _mapper);
        }

        [Fact]
        public async Task GetFinancialSpacesUserIsMemberOrOwnerOf_ReturnsMappedFinancialSpaceDtos()
        {
            // Arrange
            var userId = 1;
            var financialSpaceEntities = new List<FinancialSpaceEntity>
            {
                new FinancialSpaceEntity
                {
                    Id = 1,
                    Name = "Space 1",
                    Description = "Description 1",
                    OwnerId = userId
                },
                new FinancialSpaceEntity
                {
                    Id = 2,
                    Name = "Space 2",
                    Description = "Description 2",
                    OwnerId = userId
                }
            };

            // Mock the repository method
            _mockFinancialSpaceRepository
                .Setup(repo => repo.GetSpacesUserIsMemberOrOwnerOf(userId))
                .ReturnsAsync(financialSpaceEntities);

            // Act
            var result = await _service.GetFinancialSpacesUserIsMemberOrOwnerOf(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count()); // Ensuring that two spaces are returned

            var firstSpace = result.First();
            Assert.Equal("Space 1", firstSpace.Name);
            Assert.Equal("Description 1", firstSpace.Description);

            var secondSpace = result.Skip(1).First();
            Assert.Equal("Space 2", secondSpace.Name);
            Assert.Equal("Description 2", secondSpace.Description);
        }

        [Fact]
        public async Task GetFinancialSpacesUserIsMemberOrOwnerOf_ReturnsEmptyList_WhenNoSpacesFound()
        {
            // Arrange
            var userId = 999; // Using a non-existing user ID
            _mockFinancialSpaceRepository
                .Setup(repo => repo.GetSpacesUserIsMemberOfAsync(userId))
                .ReturnsAsync(new List<FinancialSpaceEntity>());

            // Act
            var result = await _service.GetFinancialSpacesUserIsMemberOrOwnerOf(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result); // Ensuring that the result is empty
        }

        [Fact]
        public async Task GetFinancialSpaceByIdAsync_ReturnsMappedFinancialSpaceDto()
        {
            // Arrange
            var spaceId = 1;
            var financialSpaceEntity = new FinancialSpaceEntity
            {
                Id = spaceId,
                Name = "Test Space",
                Description = "Test Description",
                OwnerId = 1,
                Owner = new UserEntity { Id = 1, FirstName = "Test", LastName = "User" }
            };

            // Mock the repository to return a space
            _mockFinancialSpaceRepository
                .Setup(repo => repo.GetFinancialSpaceAsync(spaceId))
                .ReturnsAsync(financialSpaceEntity);

            // Mock the financialGoalService to return at least one financial goal
            _mockFinancialGoalSpaceService
                .Setup(service => service.GetFinancialGoalsBySpaceIdAsync(spaceId))
                .ReturnsAsync(new List<FinancialGoalDto>
                {
                    new FinancialGoalDto
                    {
                        Id = 1,
                        Name = "Test Goal"
                    }
                });

            // Mock the repository method
            _mockFinancialSpaceRepository
                .Setup(repo => repo.GetFinancialSpaceAsync(spaceId))
                .ReturnsAsync(financialSpaceEntity);

            // Act
            var result = await _service.GetFinancialSpaceByIdAsync(spaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Space", result.Name);
            Assert.Equal("Test Description", result.Description);
            Assert.Single(result.Goals);
            Assert.Equal("Test Goal", result.Goals.First().Name);
        }

        [Fact]
        public async Task GetFinancialSpaceByIdAsync_ReturnsNull_WhenSpaceNotFound()
        {
            // Arrange
            var spaceId = 999; // Using a non-existing space ID
            _mockFinancialSpaceRepository
                .Setup(repo => repo.GetFinancialSpaceAsync(spaceId))
                .ReturnsAsync((FinancialSpaceEntity)null);

            // Act
            var result = await _service.GetFinancialSpaceByIdAsync(spaceId);

            // Assert
            Assert.Null(result); // Ensuring that the result is null
        }
    }
}
