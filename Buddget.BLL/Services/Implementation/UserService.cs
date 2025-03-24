using AutoMapper;
using Buddget.BLL.DTOs;
using Buddget.BLL.Exceptions;
using Buddget.BLL.Services.Interfaces;
using Buddget.BLL.Utilities;
using Buddget.DAL.Repositories.Interfaces;

namespace Buddget.BLL.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            return await _userRepository.Exists(userId);
        }

        public async Task<Result<UserDto>> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return Result<UserDto>.FailureResult($"User with ID {userId} not found.");
            }

            var userDto = _mapper.Map<UserDto>(user);
            return Result<UserDto>.SuccessResult(userDto);
        }

        public async Task<Result<UserDto>> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return Result<UserDto>.FailureResult($"User with email '{email}' not found.");
            }

            var userDto = _mapper.Map<UserDto>(user);
            return Result<UserDto>.SuccessResult(userDto);
        }
    }
}
