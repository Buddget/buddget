using Buddget.BLL.Services.Interfaces;
using BuddgetWeb.Areas.User.Models;
using Microsoft.AspNetCore.Mvc;

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

        public async Task<IActionResult> AccountSettings(int userId = 1)
        {
            var userExists = await _userService.UserExistsAsync(userId);
            if (!userExists)
            {
                _logger.LogWarning($"User with ID {userId} does not exist.");
                return NotFound("User not found.");
            }

            var userResult = await _userService.GetUserByIdAsync(userId);
            if (!userResult.Success)
            {
                _logger.LogWarning(userResult.ErrorMessage);
                return NotFound(userResult.ErrorMessage);
            }

            var categories = await _categoryService.GetCustomCategoriesByUserIdAsync(userId);

            var accountSettingsViewModel = new AccountSettingsViewModel
            {
                User = userResult.Value,
                Categories = categories,
            };

            _logger.LogDebug("Returning AccountSettingsViewModel");
            return View(accountSettingsViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] AddCategoryRequest request)
        {
            int userId = 1; // PLUG
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

        [HttpDelete]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            int userId = 1; // PLUG
            var success = await _categoryService.DeleteCustomCategoryAsync(userId, id);
            if (success)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Category not found." });
            }
        }

        public class AddCategoryRequest
        {
            public string CategoryName { get; set; }
        }
    }
}
