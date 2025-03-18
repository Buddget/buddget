using Buddget.DAL.DataAccess;
using Buddget.DAL.Entities;
using Buddget.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Buddget.DAL.Repositories.Implementations
{
    public class UserRepository : Repository<UserEntity>, IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<UserEntity> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<UserEntity> CreateAsync(UserEntity entity)
        {
            var createdEntity = await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return createdEntity.Entity;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id);
        }
    }
}