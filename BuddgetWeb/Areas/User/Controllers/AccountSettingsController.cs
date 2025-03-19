using Buddget.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BuddgetWeb.Areas.User.Controllers
{
    [Area("User")]
    public class AccountSettingsController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IUserService _userService;
        private readonly ILogger<AccountSettingsController> _logger;

        public AccountSettingsController(ICategoryService categoryService, IUserService userService, ILogger<AccountSettingsController> logger)
        {
            _categoryService = categoryService;
            _userService = userService;
            _logger = logger;
        }

        public async Task<IActionResult> CustomCategories(int userId = 1)
        {
            var userExists = await _userService.UserExistsAsync(userId);
            if (!userExists)
            {
                _logger.LogWarning($"User with ID {userId} does not exist.");
                return NotFound("User not found.");
            }

            var categories = await _categoryService.GetCustomCategoriesByUserIdAsync(userId);
            return View(categories);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] AddCategoryRequest request)
        {
            int userId = 1;
            var success = await _categoryService.AddCustomCategoryAsync(userId, request.CategoryName);
            if (success)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Category already exists." });
            }
        }

        public class AddCategoryRequest
        {
            public string CategoryName { get; set; }
        }
    }
}
