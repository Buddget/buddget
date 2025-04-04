using AutoMapper;
using Buddget.BLL.Enums;
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
        public async Task<IActionResult> Index(int id, TransactionSortColumnEnum sortColumn = TransactionSortColumnEnum.Id, bool ascending = true)
        {
            var financialSpace = await _financialSpaceService.GetFinancialSpaceByIdAsync(id);

            if (financialSpace == null)
            {
                _logger.LogWarning($"Trying to retrieve transactions for financial space. Financial space with id {id} not found");
                return View("NotFound");
            }

            var transactions = await _transactionService.GetSortedTransactionsBySpaceIdAsync(id, sortColumn, ascending);
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "1");
            var userSpaces = await _financialSpaceService.GetFinancialSpacesUserIsMemberOrOwnerOf(userId);

            _logger.LogInformation($"Tried retrieving transactions for space with ID {id}. Retrieved {transactions.Count()} unique transactions");

            return View(new TransactionsViewModel
            {
                Transactions = transactions,
                FinancialSpaceId = id,
                FinancialSpaceName = financialSpace.Name,
                FinancialSpaceOwner = financialSpace.OwnerName,
                SortColumn = sortColumn,
                Ascending = ascending,
                UserSpaces = userSpaces,
            });
        }

        [HttpPost]
        [Route("User/Transactions/Delete")]
        public async Task<IActionResult> Delete(int transactionId, int financialSpaceId)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "1");

            _logger.LogInformation($"User {userId} attempting to delete transaction {transactionId} from financial space {financialSpaceId}");

            var resultMessage = await _transactionService.DeleteTransactionAsync(transactionId, userId);

            TempData["Message"] = resultMessage;

            return RedirectToAction(nameof(Index), new { id = financialSpaceId });
        }

        [HttpPost]
        [Route("User/Transactions/Move")]
        public async Task<IActionResult> Move(int transactionId, int targetSpaceId)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "1");

            _logger.LogInformation($"User {userId} attempting to move transaction {transactionId} to financial space {targetSpaceId}");

            var resultMessage = await _transactionService.MoveTransactionAsync(transactionId, targetSpaceId, userId);

            TempData["Message"] = resultMessage;

            return RedirectToAction(nameof(Index), new { id = targetSpaceId });
        }
    }
}
