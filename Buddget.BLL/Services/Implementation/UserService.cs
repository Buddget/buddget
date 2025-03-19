using Buddget.BLL.Services.Interfaces;
using Buddget.DAL.Repositories.Interfaces;

namespace Buddget.BLL.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            return await _userRepository.Exists(userId);
        }
    }
}
