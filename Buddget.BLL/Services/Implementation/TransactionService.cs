using AutoMapper;
using Buddget.BLL.DTOs;
using Buddget.BLL.Services.Interfaces;
using Buddget.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Buddget.BLL.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(ITransactionRepository transactionRepository, IMapper mapper, ILogger<TransactionService> logger)
        {
            _transactionRepository = transactionRepository;
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
    }
}
