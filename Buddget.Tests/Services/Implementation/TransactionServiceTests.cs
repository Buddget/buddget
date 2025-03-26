using AutoMapper;
using Buddget.BLL.DTOs;
using Buddget.BLL.Services.Implementations;
using Buddget.DAL.Entities;
using Buddget.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Buddget.Tests.Services.Implementation
{
    public class TransactionServiceTests
    {
        private readonly Mock<ITransactionRepository> _mockTransactionRepository;
        private readonly Mock<IFinancialSpaceRepository> _mockFinancialSpaceRepository;
        private readonly IMapper _mapper;
        private readonly TransactionService _transactionService;
        private readonly Mock<ILogger<TransactionService>> _mockLogger;

        public TransactionServiceTests()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TransactionEntity, TransactionDto>().ReverseMap();
            });
            _mapper = mapperConfig.CreateMapper();

            _mockTransactionRepository = new Mock<ITransactionRepository>();
            _mockFinancialSpaceRepository = new Mock<IFinancialSpaceRepository>();
            _mockLogger = new Mock<ILogger<TransactionService>>();

            _transactionService = new TransactionService(
                _mockTransactionRepository.Object,
                _mockFinancialSpaceRepository.Object,
                _mapper,
                _mockLogger.Object);
        }

        [Fact]
        public async Task GetTransactionsBySpaceIdAsync_ReturnsTransactionDtos()
        {
            // Arrange
            int spaceId = 1;
            var transactions = new List<TransactionEntity>
            {
                new TransactionEntity
                {
                    Id = 1,
                    Name = "Transaction 1",
                    Amount = 100,
                    Currency = "USD",
                    Date = DateTime.UtcNow,
                    Description = "Description 1",
                    Category = new CategoryEntity { Name = "Category 1" },
                    User = new UserEntity { FirstName = "John", LastName = "Doe" },
                    Type = "Income"
                },
                new TransactionEntity
                {
                    Id = 2,
                    Name = "Transaction 2",
                    Amount = 200,
                    Currency = "EUR",
                    Date = DateTime.UtcNow,
                    Description = "Description 2",
                    Category = new CategoryEntity { Name = "Category 2" },
                    User = new UserEntity { FirstName = "Jane", LastName = "Doe" },
                    Type = "Expense"
                }
            };

            _mockTransactionRepository
                .Setup(repo => repo.GetTransactionsBySpaceIdAsync(spaceId))
                .ReturnsAsync(transactions);

            // Act
            var result = await _transactionService.GetTransactionsBySpaceIdAsync(spaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("Transaction 1", result.First().Name);
            Assert.Equal("Transaction 2", result.Last().Name);
        }

        [Fact]
        public async Task GetTransactionsBySpaceIdAsync_ReturnsEmptyList_WhenNoTransactionsFound()
        {
            // Arrange
            int spaceId = 1;
            _mockTransactionRepository
                .Setup(repo => repo.GetTransactionsBySpaceIdAsync(spaceId))
                .ReturnsAsync(new List<TransactionEntity>());

            // Act
            var result = await _transactionService.GetTransactionsBySpaceIdAsync(spaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task DeleteTransactionAsync_SuccessfullyDeletesTransaction()
        {
            // Arrange
            int transactionId = 1;
            int userId = 1;
            var transaction = new TransactionEntity
            {
                Id = transactionId,
                UserId = userId,
                FinancialSpaceId = 1
            };
            var financialSpace = new FinancialSpaceEntity
            {
                Id = 1,
                OwnerId = userId
            };

            _mockTransactionRepository.Setup(repo => repo.GetByIdAsync(transactionId)).ReturnsAsync(transaction);
            _mockFinancialSpaceRepository.Setup(repo => repo.GetFinancialSpaceAsync(transaction.FinancialSpaceId)).ReturnsAsync(financialSpace);
            _mockTransactionRepository.Setup(repo => repo.DeleteAsync(transaction)).Returns(Task.CompletedTask);

            // Act
            var result = await _transactionService.DeleteTransactionAsync(transactionId, userId);

            // Assert
            Assert.Equal("Transaction deleted successfully.", result);
            _mockTransactionRepository.Verify(repo => repo.DeleteAsync(transaction), Times.Once);
        }

        [Fact]
        public async Task DeleteTransactionAsync_ReturnsError_WhenTransactionNotFound()
        {
            // Arrange
            int transactionId = 1;
            int userId = 1;

            _mockTransactionRepository.Setup(repo => repo.GetByIdAsync(transactionId)).ReturnsAsync((TransactionEntity)null);

            // Act
            var result = await _transactionService.DeleteTransactionAsync(transactionId, userId);

            // Assert
            Assert.Equal("Transaction not found.", result);
            _mockTransactionRepository.Verify(repo => repo.GetByIdAsync(transactionId), Times.Once);
        }

        [Fact]
        public async Task DeleteTransactionAsync_ReturnsError_WhenUserNotAuthorized()
        {
            // Arrange
            int transactionId = 1;
            int userId = 1;
            var transaction = new TransactionEntity
            {
                Id = transactionId,
                UserId = 2,
                FinancialSpaceId = 1
            };
            var financialSpace = new FinancialSpaceEntity
            {
                Id = 1,
                OwnerId = 3
            };

            _mockTransactionRepository.Setup(repo => repo.GetByIdAsync(transactionId)).ReturnsAsync(transaction);
            _mockFinancialSpaceRepository.Setup(repo => repo.GetFinancialSpaceAsync(transaction.FinancialSpaceId)).ReturnsAsync(financialSpace);

            // Act
            var result = await _transactionService.DeleteTransactionAsync(transactionId, userId);

            // Assert
            Assert.Equal("You are not authorized to delete this transaction.", result);
            _mockTransactionRepository.Verify(repo => repo.GetByIdAsync(transactionId), Times.Once);
        }
    }
}
