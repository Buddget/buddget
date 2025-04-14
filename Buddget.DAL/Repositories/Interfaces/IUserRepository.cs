using Buddget.Domain.Entities;

namespace Buddget.DAL.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<UserEntity>
    {
        Task<bool> Exists(int id);
        Task<UserEntity> GetByEmailAsync(string email);
    }
}