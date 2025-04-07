using Buddget.BLL.DTOs;
using Buddget.BLL.Enums;

namespace Buddget.BLL.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionDto>> GetTransactionsBySpaceIdAsync(int spaceId);
        Task<string> DeleteTransactionAsync(int transactionId, int userId);
        Task<IEnumerable<TransactionDto>> GetSortedTransactionsBySpaceIdAsync(int spaceId, TransactionSortColumnEnum sortColumn = TransactionSortColumnEnum.Id, bool ascending = true);
        Task<string> MoveTransactionAsync(int transactionId, int targetSpaceId, int userId);
        Task<string> CreateTransactionAsync(TransactionDto transactionDto); // Новий метод для створення транзакції
    }
}