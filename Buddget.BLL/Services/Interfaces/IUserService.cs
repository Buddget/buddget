using Buddget.BLL.DTOs;
using Buddget.BLL.Utilities;

namespace Buddget.BLL.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> UserExistsAsync(int userId);
        Task<Result<UserDto>> GetUserByIdAsync(int userId);
        Task<Result<UserDto>> GetUserByEmailAsync(string email);
    }
}
