using AutoMapper;
using Buddget.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BuddgetWeb.Areas.User.Controllers
{
    [Area("User")]
    public class TransactionController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly IFinancialSpaceService _financialSpaceService;
        private readonly IMapper _mapper;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(ITransactionService transactionService, IFinancialSpaceService financialSpaceService, IMapper mapper, ILogger<TransactionController> logger)
        {
            _transactionService = transactionService;
            _financialSpaceService = financialSpaceService;
            _mapper = mapper;
            _logger = logger;
        }

        [Route("User/Transactions/TransactionHistory/[action]/{id?}")]
        public async Task<IActionResult> Index(int id)
        {
            var financialSpace = await _financialSpaceService.GetFinancialSpaceByIdAsync(id);

            if (financialSpace == null)
            {
                _logger.LogWarning($"Trying to retrieve trandsactions with for financial space. Financial space with id {id} not found");
                return View("NotFound");
            }

            var transactions = await _transactionService.GetTransactionsBySpaceIdAsync(id);

            _logger.LogInformation($"Tried retrieving transactions for space with ID {id}. Retrieved {transactions.Count()} unique transactions");

            return View(new TransactionsViewModel
            {
                Transactions = transactions,
                FinancialSpaceId = id,
                FinancialSpaceName = financialSpace.Name,
                FinancialSpaceOwner = financialSpace.OwnerName,
            });
        }
    }
}
