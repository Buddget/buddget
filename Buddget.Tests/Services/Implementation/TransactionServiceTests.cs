using Moq;
using AutoMapper;
using Buddget.BLL.Services.Implementations;
using Buddget.BLL.DTOs;
using Buddget.DAL.Repositories.Interfaces;
using Buddget.BLL.Mappers;
using Buddget.DAL.Entities;

namespace Buddget.BLL.Tests
{
    public class TransactionServiceTests
    {
        private readonly Mock<ITransactionRepository> _mockTransactionRepository;
        private readonly IMapper _mapper;
        private readonly TransactionService _transactionService;

        public TransactionServiceTests()
        {
            _mockTransactionRepository = new Mock<ITransactionRepository>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new TransactionProfile());
            });
            _mapper = mapperConfig.CreateMapper();

            _transactionService = new TransactionService(_mockTransactionRepository.Object, _mapper);
        }

        [Fact]
        public async Task GetTransactionsBySpaceIdAsync_ShouldReturnMappedTransactions()
        {
            // Arrange
            var spaceId = 1;
            var transactions = new List<TransactionEntity>
            {
                new TransactionEntity
                {
                    Id = 1,
                    Name = "Transaction 1",
                    Amount = 100.50m,
                    Currency = "USD",
                    Date = System.DateTime.Now,
                    Category = new CategoryEntity { Name = "Category 1" }
                },
                new TransactionEntity
                {
                    Id = 2,
                    Name = "Transaction 2",
                    Amount = 200.75m,
                    Currency = "USD",
                    Date = System.DateTime.Now,
                    Category = new CategoryEntity { Name = "Category 2" }
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
            Assert.All(result, transaction => Assert.IsType<TransactionDto>(transaction));

            var transactionList = result.ToList();
            Assert.Equal("Transaction 1", transactionList[0].Name);
            Assert.Equal("USD", transactionList[0].Currency);
            Assert.Equal("Category 1", transactionList[0].CategoryName);

            Assert.Equal("Transaction 2", transactionList[1].Name);
            Assert.Equal("USD", transactionList[1].Currency);
            Assert.Equal("Category 2", transactionList[1].CategoryName);
        }

        [Fact]
        public async Task GetTransactionsBySpaceIdAsync_ShouldReturnEmptyList_WhenNoTransactionsFound()
        {
            // Arrange
            var spaceId = 2;

            _mockTransactionRepository
                .Setup(repo => repo.GetTransactionsBySpaceIdAsync(spaceId))
                .ReturnsAsync(new List<TransactionEntity>());

            // Act
            var result = await _transactionService.GetTransactionsBySpaceIdAsync(spaceId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
