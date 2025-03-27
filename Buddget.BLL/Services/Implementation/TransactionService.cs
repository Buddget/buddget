using AutoMapper;
using Buddget.BLL.DTOs;
using Buddget.BLL.Enums;
using Buddget.BLL.Services.Interfaces;
using Buddget.DAL.Entities;
using Buddget.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Buddget.BLL.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IFinancialSpaceRepository _financialSpaceRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(ITransactionRepository transactionRepository, IFinancialSpaceRepository financialSpaceRepository, IMapper mapper, ILogger<TransactionService> logger)
        {
            _transactionRepository = transactionRepository;
            _financialSpaceRepository = financialSpaceRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<TransactionDto>> GetTransactionsBySpaceIdAsync(int spaceId)
        {
            var transactions = await _transactionRepository.GetTransactionsBySpaceIdAsync(spaceId);

            var transactionDtos = new List<TransactionDto>();

            foreach (var transaction in transactions)
            {
                var dto = new TransactionDto
                {
                    Id = transaction.Id,
                    Name = transaction.Name,
                    Amount = transaction.Amount,
                    Currency = transaction.Currency,
                    Date = transaction.Date,
                    Description = transaction.Description ?? string.Empty,
                    CategoryName = transaction.Category?.Name ?? string.Empty,
                    AuthorName = transaction.User != null
                        ? $"{transaction.User.FirstName} {transaction.User.LastName}"
                        : "Unknown",
                    Type = transaction.Type,
                };

                transactionDtos.Add(dto);
            }

            return transactionDtos;
        }

        public async Task<string> DeleteTransactionAsync(int transactionId, int userId)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (transaction == null)
            {
                _logger.LogWarning("Transaction with ID {TransactionId} was not found.", transactionId);
                return "Transaction not found.";
            }

            var financialSpace = await _financialSpaceRepository.GetFinancialSpaceAsync(transaction.FinancialSpaceId);
            if (financialSpace == null)
            {
                _logger.LogWarning("Financial space with ID {SpaceId} was not found.", transaction.FinancialSpaceId);
                return "Financial space not found.";
            }

            if (transaction.UserId != userId && financialSpace.OwnerId != userId)
            {
                _logger.LogWarning("User with ID {UserId} is not authorized to delete transaction with ID {TransactionId}.", userId, transactionId);
                return "You are not authorized to delete this transaction.";
            }

            try
            {
                await _transactionRepository.DeleteAsync(transaction);
                _logger.LogInformation("Successfully deleted transaction with ID {TransactionId}.", transactionId);
                return "Transaction deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting to delete transaction with ID {TransactionId}.", transactionId);
                return "An error occurred while deleting the transaction.";
            }
        }

        public async Task<IEnumerable<TransactionDto>> GetSortedTransactionsBySpaceIdAsync(int spaceId, TransactionSortColumnEnum sortColumn = TransactionSortColumnEnum.Id, bool ascending = true)
        {
            var transactions = await GetTransactionsBySpaceIdAsync(spaceId);
            var sortedTransactions = SortTransactions(transactions, sortColumn, ascending);
            _logger.LogInformation($"Retrieved {sortedTransactions.Count()} transactions for space with ID {spaceId}. Sorted by {sortColumn} in {(ascending ? "ascending" : "descending")} order.");
            return _mapper.Map<IEnumerable<TransactionDto>>(sortedTransactions);
        }

        public async Task<string> MoveTransactionAsync(int transactionId, int targetSpaceId, int userId)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (transaction == null)
            {
                _logger.LogWarning("Transaction not found");
                return "Transaction not found.";
            }

            var currentSpace = await _financialSpaceRepository.GetFinancialSpaceAsync(transaction.FinancialSpaceId);
            var targetSpace = await _financialSpaceRepository.GetFinancialSpaceAsync(targetSpaceId);

            if (currentSpace == null || targetSpace == null)
            {
                _logger.LogWarning("One or both of the financial spaces were not found.");
                return "One or both of the financial spaces were not found.";
            }

            if (currentSpace.OwnerId != userId && (currentSpace.Members == null || !currentSpace.Members.Any(m => m.UserId == userId)))
            {
                _logger.LogWarning("User is not authorized to move the transaction from this space.");
                return "You are not authorized to move the transaction from this space.";
            }

            if (targetSpace.OwnerId != userId && (targetSpace.Members == null || !targetSpace.Members.Any(m => m.UserId == userId)))
            {
                _logger.LogWarning("User is not authorized to move the transaction to the target space.");
                return "You are not authorized to move the transaction to the target space.";
            }

            transaction.FinancialSpaceId = targetSpaceId;
            await _transactionRepository.UpdateAsync(transaction);

            _logger.LogInformation("Transaction moved successfully.");
            return "Transaction moved successfully.";
        }

        private IEnumerable<TransactionDto> SortTransactions(IEnumerable<TransactionDto> transactions, TransactionSortColumnEnum sortColumn, bool ascending = true)
        {
            return sortColumn switch
            {
                TransactionSortColumnEnum.Id => ascending ? transactions.OrderBy(t => t.Id) : transactions.OrderByDescending(t => t.Id),
                TransactionSortColumnEnum.Name => ascending ? transactions.OrderBy(t => t.Name) : transactions.OrderByDescending(t => t.Name),
                TransactionSortColumnEnum.Amount => ascending ? transactions.OrderBy(t => t.Amount) : transactions.OrderByDescending(t => t.Amount),
                TransactionSortColumnEnum.Currency => ascending ? transactions.OrderBy(t => t.Currency) : transactions.OrderByDescending(t => t.Currency),
                TransactionSortColumnEnum.Date => ascending ? transactions.OrderBy(t => t.Date) : transactions.OrderByDescending(t => t.Date),
                TransactionSortColumnEnum.Type => ascending ? transactions.OrderBy(t => t.Type) : transactions.OrderByDescending(t => t.Type),
                TransactionSortColumnEnum.CategoryName => ascending ? transactions.OrderBy(t => t.CategoryName) : transactions.OrderByDescending(t => t.CategoryName),
                TransactionSortColumnEnum.AuthorName => ascending ? transactions.OrderBy(t => t.AuthorName) : transactions.OrderByDescending(t => t.AuthorName),
                TransactionSortColumnEnum.Description => ascending ? transactions.OrderBy(t => t.Description) : transactions.OrderByDescending(t => t.Description),
                _ => transactions
            };
        }
    }
}
