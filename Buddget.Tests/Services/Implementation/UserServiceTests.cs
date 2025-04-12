using Moq;
using Buddget.BLL.Services.Implementations;
using Buddget.DAL.Repositories.Interfaces;
using Buddget.Domain.Entities;
using AutoMapper;
using Buddget.BLL.Mappers;
using Buddget.BLL.DTOs;
using Buddget.BLL.Exceptions;


namespace Buddget.Tests.Services.Implementation
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly IMapper _mapper;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new UserProfile());
            });
            _mapper = mapperConfig.CreateMapper();
            _userService = new UserService(_mockUserRepository.Object, _mapper);
        }

        [Fact]
        public async Task UserExistsAsync_UserExists_ReturnsTrue()
        {
            // Arrange
            int userId = 1;
            _mockUserRepository.Setup(repo => repo.Exists(userId)).ReturnsAsync(true);

            // Act
            var result = await _userService.UserExistsAsync(userId);

            // Assert
            Assert.True(result);
            _mockUserRepository.Verify(repo => repo.Exists(userId), Times.Once);
        }

        [Fact]
        public async Task UserExistsAsync_UserDoesNotExist_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            _mockUserRepository.Setup(repo => repo.Exists(userId)).ReturnsAsync(false);

            // Act
            var result = await _userService.UserExistsAsync(userId);

            // Assert
            Assert.False(result);
            _mockUserRepository.Verify(repo => repo.Exists(userId), Times.Once);
        }

        [Fact]
        public async Task GetUserByIdAsync_UserExists_ReturnsUserDto()
        {
            // Arrange
            int userId = 1;
            var userEntity = new UserEntity
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                //Role = "user",
                RegisteredAt = DateTime.UtcNow
            };

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(userEntity);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);
            var userDto = result.Value;

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.Equal("john.doe@example.com", result.Value.Email);
            Assert.Equal("John", result.Value.FirstName);
            Assert.Equal("Doe", result.Value.LastName);
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetUserByIdAsync_UserDoesNotExist_ReturnsFailureResult()
        {
            // Arrange
            int userId = 1;
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((UserEntity)null);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal($"User with ID {userId} not found.", result.ErrorMessage);
            _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetUserByEmailAsync_UserExists_ReturnsUserDto()
        {
            // Arrange
            string email = "john.doe@example.com";
            var userEntity = new UserEntity
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = email,
                //Role = "user",
                RegisteredAt = DateTime.UtcNow
            };

            _mockUserRepository.Setup(repo => repo.GetByEmailAsync(email)).ReturnsAsync(userEntity);

            // Act
            var result = await _userService.GetUserByEmailAsync(email);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.Equal(email, result.Value.Email);
            Assert.Equal("John", result.Value.FirstName);
            Assert.Equal("Doe", result.Value.LastName);
            _mockUserRepository.Verify(repo => repo.GetByEmailAsync(email), Times.Once);
        }

        [Fact]
        public async Task GetUserByEmailAsync_UserDoesNotExist_ReturnsFailureResult()
        {
            // Arrange
            string email = "nonexistent@example.com";
            _mockUserRepository.Setup(repo => repo.GetByEmailAsync(email)).ReturnsAsync((UserEntity)null);

            // Act
            var result = await _userService.GetUserByEmailAsync(email);

            // Assert
            Assert.False(result.Success);
            Assert.Equal($"User with email '{email}' not found.", result.ErrorMessage);
            _mockUserRepository.Verify(repo => repo.GetByEmailAsync(email), Times.Once);
        }

    }
}
