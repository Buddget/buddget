using Moq;
using Buddget.BLL.Services.Implementations;
using Buddget.DAL.Repositories.Interfaces;


namespace Buddget.Tests.Services.Implementation
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _userService = new UserService(_mockUserRepository.Object);
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
    }
}
