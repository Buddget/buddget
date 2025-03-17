using Buddget.BLL.DTOs;

namespace Buddget.BLL.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionDto>> GetTransactionsBySpaceIdAsync(int spaceId);
    }
}
