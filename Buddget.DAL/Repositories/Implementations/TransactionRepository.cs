using Buddget.DAL.DataAccess;
using Buddget.DAL.Entities;
using Buddget.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Buddget.DAL.Repositories.Implementations
{
    public class TransactionRepository : Repository<TransactionEntity>, ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TransactionEntity>> GetTransactionsByUserIdAsync(int userId)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<TransactionEntity>> GetTransactionsBySpaceIdAsync(int spaceId)
        {
            return await _context.Transactions
                .Where(t => t.FinancialSpaceId == spaceId)
                .ToListAsync();
        }

        public async Task<IEnumerable<TransactionEntity>> GetTransactionsByCategoryIdAsync(int categoryId)
        {
            return await _context.Transactions
                .Where(t => t.CategoryId == categoryId)
                .ToListAsync();
        }
    }
}