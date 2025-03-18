using Moq;
using Buddget.BLL.Services.Implementation;
using Buddget.DAL.Repositories.Interfaces;
using AutoMapper;
using Buddget.DAL.Entities;
using Buddget.BLL.Mappers;

public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly IMapper _mapper;
        private readonly UserService _service;

        public UserServiceTests()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new UserProfile());
            });
            _mapper = mapperConfig.CreateMapper();

            _mockUserRepository = new Mock<IUserRepository>();
            _service = new UserService(_mockUserRepository.Object, _mapper);
        }

        [Fact]
        public async Task CreateAsync_CreatesAndReturnsUser()
        {
            var userDto = new Buddget.BLL.DTOs.UserDto { Id = 1, Email = "test@example.com", FirstName = "John", LastName = "Doe" };
            var entity = _mapper.Map<UserEntity>(userDto);

            _mockUserRepository.Setup(repo => repo.CreateAsync(It.IsAny<UserEntity>())).ReturnsAsync(entity);

            var result = await _service.CreateAsync(userDto);

            Assert.NotNull(result);
            Assert.Equal(userDto.Email, result.Email);
            _mockUserRepository.Verify(repo => repo.CreateAsync(It.IsAny<UserEntity>()), Times.Once);
        }

        [Fact]
        public async Task GetByEmailAsync_ReturnsUser_WhenUserExists()
        {
            var userEntity = new UserEntity { Id = 1, Email = "test@example.com", FirstName = "John", LastName = "Doe" };
            _mockUserRepository.Setup(repo => repo.GetByEmailAsync(userEntity.Email)).ReturnsAsync(userEntity);

            var result = await _service.GetByEmailAsync(userEntity.Email);

            Assert.NotNull(result);
            Assert.Equal(userEntity.Email, result.Email);
        }
    }