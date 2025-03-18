using AutoMapper;
using Buddget.BLL.DTOs;
using Buddget.BLL.Services.Interfaces;
using Buddget.DAL.Entities;
using Buddget.DAL.Repositories.Interfaces;

namespace Buddget.BLL.Services.Implementation
{
    public class UserService(
        IUserRepository userRepository,
        IMapper mapper): IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<UserDto> CreateAsync(UserDto userDto)
        {
            UserEntity entity = _mapper.Map<UserEntity>(userDto);
            var created = await _userRepository.CreateAsync(entity);
            return _mapper.Map<UserDto>(created);
        }
    }
}
