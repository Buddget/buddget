using AutoMapper;
using Buddget.BLL.DTOs;
using Buddget.BLL.Services.Interfaces;
using Buddget.DAL.Repositories.Implementations;
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
    }
}
