using AutoMapper;
using Moq;
using Buddget.BLL.DTOs;
using Buddget.BLL.Services.Implementations;
using Buddget.Domain.Entities;
using Buddget.DAL.Repositories.Interfaces;
using Buddget.BLL.Mappers;
using Buddget.BLL.Services.Interfaces;
using Microsoft.Extensions.Logging;

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
        private readonly Mock<ILogger<FinancialSpaceService>> _mockLogger;

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
            _mockLogger = new Mock<ILogger<FinancialSpaceService>>();

            // Create the service with mocked dependencies
            _service = new FinancialSpaceService(
                _mockFinancialSpaceRepository.Object,
                _mockFinancialSpaceMemberService.Object,
                _mockFinancialGoalSpaceService.Object,
                _mockTransactionService.Object,
            _mapper,
                _mockLogger.Object);
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
        [Fact]
        public async Task DeleteFinancialSpaceAsync_ReturnsSuccessMessage_WhenSpaceIsDeletedSuccessfully()
        {
            // Arrange
            var userId = 1;
            var spaceId = 1;
            var financialSpaceEntity = new FinancialSpaceEntity
            {
                Id = spaceId,
                Name = "Test Space",
                Description = "Test Description",
                OwnerId = userId,
            };

            // Mock the repository to return the financial space entity
            _mockFinancialSpaceRepository
                .Setup(repo => repo.GetFinancialSpaceAsync(spaceId))
                .ReturnsAsync(financialSpaceEntity);

            // Mock the DeleteAsync method to ensure it is called successfully
            _mockFinancialSpaceRepository
                .Setup(repo => repo.DeleteAsync(financialSpaceEntity))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteFinancialSpaceAsync(userId, spaceId);

            // Assert
            Assert.Equal("Financial space deleted successfully.", result); // Ensuring the success message is returned
            _mockFinancialSpaceRepository.Verify(repo => repo.DeleteAsync(financialSpaceEntity), Times.Once); // Verify that DeleteAsync was called once
        }
        [Fact]
        public async Task DeleteFinancialSpaceAsync_ReturnsNotFoundMessage_WhenSpaceNotFound()
        {
            // Arrange
            var userId = 1;
            var spaceId = 999; // Using a non-existing space ID
            _mockFinancialSpaceRepository
                .Setup(repo => repo.GetFinancialSpaceAsync(spaceId))
                .ReturnsAsync((FinancialSpaceEntity)null); // Return null to simulate the space not found

            // Act
            var result = await _service.DeleteFinancialSpaceAsync(userId, spaceId);

            // Assert
            Assert.Equal("Financial space not found.", result); // Ensuring that the correct message is returned
        }
        [Fact]
        public async Task DeleteFinancialSpaceAsync_ReturnsOwnershipErrorMessage_WhenUserIsNotOwner()
        {
            // Arrange
            var userId = 2; // User who is not the owner
            var spaceId = 1;
            var financialSpaceEntity = new FinancialSpaceEntity
            {
                Id = spaceId,
                Name = "Test Space",
                Description = "Test Description",
                OwnerId = 1, // Different owner
            };

            // Mock the repository to return the financial space entity
            _mockFinancialSpaceRepository
                .Setup(repo => repo.GetFinancialSpaceAsync(spaceId))
                .ReturnsAsync(financialSpaceEntity);

            // Act
            var result = await _service.DeleteFinancialSpaceAsync(userId, spaceId);

            // Assert
            Assert.Equal("You are not the owner of this financial space.", result); // Ensuring the correct ownership error message is returned
        }
        [Fact]
        public async Task CreateFinancialSpaceAsync_ReturnsSuccessMessage_WhenSpaceIsCreatedSuccessfully()
        {
            // Arrange
            var financialSpaceDto = new FinancialSpaceDto
            {
                Id = 1,
                Name = "Test Space",
                Description = "Test Description",
                OwnerId = 1,
                ImageName = "test.jpg",
                ImageData = new byte[] { 1, 2, 3 }
            };

            var financialSpaceEntity = new FinancialSpaceEntity
            {
                Id = 1,
                Name = "Test Space",
                Description = "Test Description",
                OwnerId = 1,
                ImageName = "test.jpg",
                ImageData = new byte[] { 1, 2, 3 },
                CreatedAt = DateTime.UtcNow
            };

            _mockFinancialSpaceRepository
                .Setup(repo => repo.GetFinancialSpaceAsync(financialSpaceDto.Id))
                .ReturnsAsync((FinancialSpaceEntity)null);

            _mockFinancialSpaceRepository
                .Setup(repo => repo.AddAsync(It.IsAny<FinancialSpaceEntity>()))
                .Returns(Task.CompletedTask);

            // Act
            var (createdSpace, resultMessage) = await _service.CreateFinancialSpaceAsync(financialSpaceDto);

            // Assert
            Assert.Equal("Financial space created successfully.", resultMessage);
            Assert.NotNull(createdSpace);
            Assert.Equal(financialSpaceDto.Name, createdSpace.Name);
            _mockFinancialSpaceRepository.Verify(repo => repo.AddAsync(It.IsAny<FinancialSpaceEntity>()), Times.Once);
        }

        [Fact]
        public async Task CreateFinancialSpaceAsync_ReturnsErrorMessage_WhenFinancialSpaceDataIsNull()
        {
            // Arrange
            FinancialSpaceDto financialSpaceDto = null;

            // Act
            var (createdSpace, resultMessage) = await _service.CreateFinancialSpaceAsync(financialSpaceDto);

            // Assert
            Assert.Equal("Financial space data is required.", resultMessage);
            Assert.Null(createdSpace);
            _mockFinancialSpaceRepository.Verify(repo => repo.AddAsync(It.IsAny<FinancialSpaceEntity>()), Times.Never);
        }

        [Fact]
        public async Task CreateFinancialSpaceAsync_ReturnsErrorMessage_WhenNameIsNull()
        {
            // Arrange
            var financialSpaceDto = new FinancialSpaceDto
            {
                Id = 1,
                Description = "Test Description",
                OwnerId = 1,
                ImageName = "test.jpg",
                ImageData = new byte[] { 1, 2, 3 }
            };

            // Act
            var (createdSpace, resultMessage) = await _service.CreateFinancialSpaceAsync(financialSpaceDto);

            // Assert
            Assert.Equal("Name is required.", resultMessage);
            Assert.Null(createdSpace);
            _mockFinancialSpaceRepository.Verify(repo => repo.AddAsync(It.IsAny<FinancialSpaceEntity>()), Times.Never);
        }

        [Fact]
        public async Task CreateFinancialSpaceAsync_ReturnsErrorMessage_WhenSpaceWithSameIdAlreadyExists()
        {
            // Arrange
            var financialSpaceDto = new FinancialSpaceDto
            {
                Id = 1,
                Name = "Test Space",
                Description = "Test Description",
                OwnerId = 1,
                ImageName = "test.jpg",
                ImageData = new byte[] { 1, 2, 3 }
            };

            var existingFinancialSpaceEntity = new FinancialSpaceEntity
            {
                Id = 1,
                Name = "Existing Space",
                Description = "Existing Description",
                OwnerId = 1,
                ImageName = "existing.jpg",
                ImageData = new byte[] { 4, 5, 6 },
                CreatedAt = DateTime.UtcNow
            };

            _mockFinancialSpaceRepository
                .Setup(repo => repo.GetFinancialSpaceAsync(financialSpaceDto.Id))
                .ReturnsAsync(existingFinancialSpaceEntity);

            // Act
            var (createdSpace, resultMessage) = await _service.CreateFinancialSpaceAsync(financialSpaceDto);

            // Assert
            Assert.Equal($"Financial space with ID={existingFinancialSpaceEntity.Id} already exists.", resultMessage);
            Assert.Null(createdSpace);
            _mockFinancialSpaceRepository.Verify(repo => repo.AddAsync(It.IsAny<FinancialSpaceEntity>()), Times.Never);
        }

        [Fact]
        public async Task CreateFinancialSpaceAsync_ReturnsErrorMessage_WhenDescriptionExceedsMaxLength()
        {
            // Arrange
            var financialSpaceDto = new FinancialSpaceDto
            {
                Id = 1,
                Name = "Test Space",
                Description = new string('a', 1001), // Description with 1001 characters
                OwnerId = 1,
                ImageName = "test.jpg",
                ImageData = new byte[] { 1, 2, 3 }
            };

            // Act
            var (createdSpace, resultMessage) = await _service.CreateFinancialSpaceAsync(financialSpaceDto);

            // Assert
            Assert.Equal("Description cannot exceed 1000 characters.", resultMessage); // Fix: Use resultMessage instead of result
            Assert.Null(createdSpace);
            _mockFinancialSpaceRepository.Verify(repo => repo.AddAsync(It.IsAny<FinancialSpaceEntity>()), Times.Never);
        }
    }
}
