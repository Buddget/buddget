using Buddget.BLL.Services.Interfaces;
using Buddget.DAL.Repositories.Interfaces;
using AutoMapper;
using Buddget.BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Buddget.BLL.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;

        public TransactionService(ITransactionRepository transactionRepository, IMapper mapper)
        {
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TransactionDto>> GetTransactionsBySpaceIdAsync(int spaceId)
        {
            var transactions = await _transactionRepository.GetTransactionsBySpaceIdAsync(spaceId);
            return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
        }
    }
}
