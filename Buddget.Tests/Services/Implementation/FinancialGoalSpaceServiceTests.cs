using AutoMapper;
using Moq;
using Buddget.BLL.Services.Implementations;
using Buddget.Domain.Entities;
using Buddget.DAL.Repositories.Interfaces;
using Buddget.BLL.Mappers;

namespace Buddget.Tests.Services.Implementation
{
    public class FinancialGoalSpaceServiceTests
    {
        private readonly Mock<IFinancialGoalSpaceRepository> _mockFinancialGoalSpaceRepository;
        private readonly IMapper _mapper;
        private readonly FinancialGoalSpaceService _service;

        public FinancialGoalSpaceServiceTests()
        {
            // Arrange AutoMapper configuration
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new FinancialGoalSpaceProfile()); // Using the profile you've set up
            });
            _mapper = mapperConfig.CreateMapper();

            // Initialize the mock repository
            _mockFinancialGoalSpaceRepository = new Mock<IFinancialGoalSpaceRepository>();

            // Create the service with mocked dependencies
            _service = new FinancialGoalSpaceService(_mockFinancialGoalSpaceRepository.Object, _mapper);
        }

        [Fact]
        public async Task GetFinancialGoalsBySpaceIdAsync_ReturnsMappedFinancialGoalDtos()
        {
            // Arrange
            var spaceId = 1;
            var financialGoalSpaceEntities = new List<FinancialGoalSpaceEntity>
            {
                new FinancialGoalSpaceEntity
                {
                    Id = 1,
                    FinancialGoalId = 1,
                    FinancialSpaceId = spaceId,
                    FinancialGoal = new FinancialGoalEntity
                    {
                        Id = 1,
                        Name = "Goal 1",
                        Type = "income",
                        TargetAmount = 1000,
                        StartDate = new DateTime(2025, 01, 01),
                        EndDate = new DateTime(2025, 12, 31)
                    }
                },
                new FinancialGoalSpaceEntity
                {
                    Id = 2,
                    FinancialGoalId = 2,
                    FinancialSpaceId = spaceId,
                    FinancialGoal = new FinancialGoalEntity
                    {
                        Id = 2,
                        Name = "Goal 2",
                        Type = "expense",
                        TargetAmount = 500,
                        StartDate = new DateTime(2025, 01, 01),
                        EndDate = new DateTime(2025, 06, 30)
                    }
                }
            };

            // Mock the repository method
            _mockFinancialGoalSpaceRepository
                .Setup(repo => repo.GetAllBySpaceIdAsync(spaceId))
                .ReturnsAsync(financialGoalSpaceEntities);

            // Act
            var result = await _service.GetFinancialGoalsBySpaceIdAsync(spaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count()); // Ensuring that two goals are returned

            var firstGoal = result.First();
            Assert.Equal("Goal 1", firstGoal.Name);
            Assert.Equal("income", firstGoal.Type);
            Assert.Equal(1000, firstGoal.TargetAmount);
            Assert.Equal(new DateTime(2025, 01, 01), firstGoal.StartDate);
            Assert.Equal(new DateTime(2025, 12, 31), firstGoal.EndDate);

            var secondGoal = result.Skip(1).First();
            Assert.Equal("Goal 2", secondGoal.Name);
            Assert.Equal("expense", secondGoal.Type);
            Assert.Equal(500, secondGoal.TargetAmount);
            Assert.Equal(new DateTime(2025, 01, 01), secondGoal.StartDate);
            Assert.Equal(new DateTime(2025, 06, 30), secondGoal.EndDate);
        }

        [Fact]
        public async Task GetFinancialGoalsBySpaceIdAsync_ReturnsEmptyList_WhenNoGoalsFound()
        {
            // Arrange
            var spaceId = 999; // Using a non-existing space ID
            _mockFinancialGoalSpaceRepository
                .Setup(repo => repo.GetAllBySpaceIdAsync(spaceId))
                .ReturnsAsync(new List<FinancialGoalSpaceEntity>());

            // Act
            var result = await _service.GetFinancialGoalsBySpaceIdAsync(spaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result); // Ensuring that the result is empty
        }
    }
}
