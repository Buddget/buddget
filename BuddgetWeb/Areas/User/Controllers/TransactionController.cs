using AutoMapper;
using Buddget.BLL.Enums;
using Buddget.BLL.DTOs;
using Buddget.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Buddget.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace BuddgetWeb.Areas.User.Controllers
{
    [Area("User")]
    public class TransactionController : Controller
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly ITransactionService _transactionService;
        private readonly IFinancialSpaceService _financialSpaceService;
        private readonly IMapper _mapper;
        private readonly ICategoryService _categoryService;

        private readonly ILogger<TransactionController> _logger;

        public TransactionController(UserManager<UserEntity> userManager ,ITransactionService transactionService,ICategoryService categoryService, IFinancialSpaceService financialSpaceService, IMapper mapper, ILogger<TransactionController> logger)
        {
            _userManager = userManager;
            _transactionService = transactionService;
            _financialSpaceService = financialSpaceService;
            _mapper = mapper;
            _categoryService = categoryService;
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
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("Failed to retrieve the current user. The user may not be authenticated.");
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            int userId = user.Id;

            var userSpaces = await _financialSpaceService.GetFinancialSpacesUserIsMemberOrOwnerOf(userId);

            _logger.LogInformation($"Tried retrieving transactions for space with ID {id}. Retrieved {transactions.Count()} unique transactions");

            var userCategories = await _categoryService.GetCustomCategoriesByUserIdAsync(userId);

            // Отримуємо дефолтні категорії
            var defaultCategories = await _categoryService.GetDefaultCategoriesAsync();

            // Об'єднуємо їх в один список, видаляємо дублікати за Id
            var allCategories = userCategories.Concat(defaultCategories).DistinctBy(c => c.Id).ToList();

            return View(new TransactionsViewModel
            {
                Transactions = transactions,
                FinancialSpaceId = id,
                FinancialSpaceName = financialSpace.Name,
                FinancialSpaceOwner = financialSpace.OwnerName,
                SortColumn = sortColumn,
                Ascending = ascending,
                UserSpaces = userSpaces,
                UserCategories = allCategories,
            });
        }

        [HttpPost]
        [Route("User/Transactions/Delete")]
        public async Task<IActionResult> Delete(int transactionId, int financialSpaceId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("Failed to retrieve the current user. The user may not be authenticated.");
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            int userId = user.Id;

            _logger.LogInformation($"User {userId} attempting to delete transaction {transactionId} from financial space {financialSpaceId}");

            var resultMessage = await _transactionService.DeleteTransactionAsync(transactionId, userId);

            TempData["Message"] = resultMessage;

            return RedirectToAction(nameof(Index), new { id = financialSpaceId });
        }

        [HttpPost]
        [Route("User/Transactions/Move")]
        public async Task<IActionResult> Move(int transactionId, int targetSpaceId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("Failed to retrieve the current user. The user may not be authenticated.");
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            int userId = user.Id;

            _logger.LogInformation($"User {userId} attempting to move transaction {transactionId} to financial space {targetSpaceId}");

            var resultMessage = await _transactionService.MoveTransactionAsync(transactionId, targetSpaceId, userId);

            TempData["Message"] = resultMessage;

            return RedirectToAction(nameof(Index), new { id = targetSpaceId });
        }

        [HttpPost]
        [Route("User/Transactions/Create")]
        public async Task<IActionResult> Create(string Name, decimal Amount, string Type, int? CategoryId, string Description, int financialSpaceId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("Failed to retrieve the current user. The user may not be authenticated.");
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            int userId = user.Id;

            _logger.LogInformation($"User {userId} creating new transaction in space {financialSpaceId}");

            var transaction = new TransactionDto
            {
                Name = Name,
                Amount = Amount,
                Type = Type,
                CategoryId = CategoryId,
                Description = Description,
                FinancialSpaceId = financialSpaceId,
                AuthorId = userId,
                Date = DateTime.UtcNow,
            };

            var resultMessage = await _transactionService.CreateTransactionAsync(transaction);

            TempData["Message"] = resultMessage;

            return RedirectToAction(nameof(Index), new { id = financialSpaceId });
        }
    }
}
