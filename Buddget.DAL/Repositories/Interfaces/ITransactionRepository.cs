using Buddget.Domain.Entities;

namespace Buddget.DAL.Repositories.Interfaces
{
    public interface ITransactionRepository : IRepository<TransactionEntity>
    {
        Task<IEnumerable<TransactionEntity>> GetTransactionsByUserIdAsync(int userId);
        Task<IEnumerable<TransactionEntity>> GetTransactionsBySpaceIdAsync(int spaceId);
        Task<IEnumerable<TransactionEntity>> GetTransactionsByCategoryIdAsync(int categoryId);
    }
}