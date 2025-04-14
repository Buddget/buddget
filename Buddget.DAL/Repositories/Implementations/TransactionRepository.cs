using Buddget.DAL.DataAccess;
using Buddget.DAL.Repositories.Interfaces;
using Buddget.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Buddget.DAL.Repositories.Implementations
{
    public class TransactionRepository : Repository<TransactionEntity>, ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TransactionEntity>> GetTransactionsByUserIdAsync(int userId)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId)
                .Include(t => t.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<TransactionEntity>> GetTransactionsBySpaceIdAsync(int spaceId)
        {
            return await _context.Set<TransactionEntity>()
                .Include(t => t.User)
                .Include(t => t.Category)
                .Where(t => t.FinancialSpaceId == spaceId)
                .ToListAsync();
        }

        public async Task<IEnumerable<TransactionEntity>> GetTransactionsByCategoryIdAsync(int categoryId)
        {
            return await _context.Transactions
                .Where(t => t.CategoryId == categoryId)
                .Include(t => t.User)
                .Include(t => t.Category)
                .ToListAsync();
        }
    }
}