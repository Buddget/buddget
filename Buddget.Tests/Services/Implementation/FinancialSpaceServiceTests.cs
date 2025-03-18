using Moq;
using AutoMapper;
using Buddget.BLL.Services.Implementations;
using Buddget.BLL.DTOs;
using Buddget.DAL.Repositories.Interfaces;
using Buddget.DAL.Entities;
using Buddget.BLL.Mappers;
using Buddget.BLL.Services.Interfaces;

namespace Buddget.BLL.Tests
{
    public class FinancialSpaceServiceTests
    {
        private readonly Mock<IFinancialSpaceRepository> _mockFinancialSpaceRepository;
        private readonly Mock<IFinancialSpaceMemberService> _mockFinancialSpaceMemberService;
        private readonly Mock<IFinancialGoalSpaceService> _mockFinancialGoalSpaceService;
        private readonly Mock<ITransactionService> _mockTransactionService;
        private readonly IMapper _mapper;
        private readonly FinancialSpaceService _financialSpaceService;

        public FinancialSpaceServiceTests()
        {
            _mockFinancialSpaceRepository = new Mock<IFinancialSpaceRepository>();
            _mockFinancialSpaceMemberService = new Mock<IFinancialSpaceMemberService>();
            _mockFinancialGoalSpaceService = new Mock<IFinancialGoalSpaceService>();
            _mockTransactionService = new Mock<ITransactionService>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new FinancialSpaceProfile()); // Ensure you have a mapping profile for FinancialSpace
            });
            _mapper = mapperConfig.CreateMapper();

            _financialSpaceService = new FinancialSpaceService(
                _mockFinancialSpaceRepository.Object,
                _mockFinancialSpaceMemberService.Object,
                _mockFinancialGoalSpaceService.Object,
                _mockTransactionService.Object,
                _mapper
            );
        }

        [Fact]
        public async Task GetFinancialSpaceByIdAsync_ShouldReturnFinancialSpaceDto_WhenSpaceExists()
        {
            // Arrange
            var spaceId = 1;
            var spaceEntity = new FinancialSpaceEntity
            {
                Id = spaceId,
                Name = "Test Space",
                Description = "Test Description",
                Owner = new UserEntity
                {
                    FirstName = "John",
                    LastName = "Doe"
                }
            };

            var members = new List<FinancialSpaceMemberDto>
            {
                new FinancialSpaceMemberDto { Email = "member1@example.com", MemberRole = "User" },
                new FinancialSpaceMemberDto { Email = "member2@example.com", MemberRole = "Admin" }
            };

            var financialGoals = new List<FinancialGoalDto>
            {
                new FinancialGoalDto { Name = "Goal 1", TargetAmount = 1000 },
                new FinancialGoalDto { Name = "Goal 2", TargetAmount = 2000 }
            };

            var transactions = new List<TransactionDto>
            {
                new TransactionDto { Name = "Transaction 1", Amount = 100, Currency = "USD" },
                new TransactionDto { Name = "Transaction 2", Amount = 200, Currency = "USD" }
            };

            _mockFinancialSpaceRepository
                .Setup(repo => repo.GetFinancialSpaceAsync(spaceId))
                .ReturnsAsync(spaceEntity);

            _mockFinancialSpaceMemberService
                .Setup(service => service.GetMembersBySpaceIdAsync(spaceId))
                .ReturnsAsync(members);

            _mockFinancialGoalSpaceService
                .Setup(service => service.GetFinancialGoalsBySpaceIdAsync(spaceId))
                .ReturnsAsync(financialGoals);

            _mockTransactionService
                .Setup(service => service.GetTransactionsBySpaceIdAsync(spaceId))
                .ReturnsAsync(transactions);

            // Act
            var result = await _financialSpaceService.GetFinancialSpaceByIdAsync(spaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(spaceEntity.Name, result.Name);
            Assert.Equal(spaceEntity.Description, result.Description);
            Assert.Equal(spaceEntity.Owner.FirstName, result.OwnerName);

            Assert.Equal(2, result.Members.Count);
            Assert.Equal("member1@example.com", result.Members[0].Email);
            Assert.Equal("member2@example.com", result.Members[1].Email);

            Assert.Equal(2, result.Goals.Count);
            Assert.Equal("Goal 1", result.Goals[0].Name);
            Assert.Equal("Goal 2", result.Goals[1].Name);

            Assert.Equal(2, result.RecentTransactions.Count);
            Assert.Equal("Transaction 1", result.RecentTransactions[0].Name);
            Assert.Equal("Transaction 2", result.RecentTransactions[1].Name);
        }

        [Fact]
        public async Task GetFinancialSpaceByIdAsync_ShouldReturnNull_WhenSpaceDoesNotExist()
        {
            // Arrange
            var spaceId = 999;

            _mockFinancialSpaceRepository
                .Setup(repo => repo.GetFinancialSpaceAsync(spaceId))
                .ReturnsAsync((FinancialSpaceEntity)null);

            // Act
            var result = await _financialSpaceService.GetFinancialSpaceByIdAsync(spaceId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetFinancialSpaceByIdAsync_ShouldReturnEmptyLists_WhenNoMembersGoalsOrTransactionsExist()
        {
            // Arrange
            var spaceId = 1;
            var spaceEntity = new FinancialSpaceEntity
            {
                Id = spaceId,
                Name = "Test Space",
                Description = "Test Description",
                Owner = new UserEntity
                {
                    FirstName = "John",
                    LastName = "Doe"
                }
            };

            _mockFinancialSpaceRepository
                .Setup(repo => repo.GetFinancialSpaceAsync(spaceId))
                .ReturnsAsync(spaceEntity);

            _mockFinancialSpaceMemberService
                .Setup(service => service.GetMembersBySpaceIdAsync(spaceId))
                .ReturnsAsync(new List<FinancialSpaceMemberDto>());

            _mockFinancialGoalSpaceService
                .Setup(service => service.GetFinancialGoalsBySpaceIdAsync(spaceId))
                .ReturnsAsync(new List<FinancialGoalDto>());

            _mockTransactionService
                .Setup(service => service.GetTransactionsBySpaceIdAsync(spaceId))
                .ReturnsAsync(new List<TransactionDto>());

            // Act
            var result = await _financialSpaceService.GetFinancialSpaceByIdAsync(spaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Members);
            Assert.Empty(result.Goals);
            Assert.Empty(result.RecentTransactions);
        }
    }
}