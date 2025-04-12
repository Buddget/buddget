using Buddget.BLL.Services.Interfaces;
using Buddget.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BuddgetWeb.Areas.Identity.Pages.Account.Manage
{
    public class DeleteCategoryModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        private readonly UserManager<UserEntity> _userManager;
        private readonly ILogger<DeleteCategoryModel> _logger;

        public DeleteCategoryModel(ICategoryService categoryService, UserManager<UserEntity> userManager, ILogger<DeleteCategoryModel> logger)
        {
            _categoryService = categoryService;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public int CategoryId { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("Unable to load user for category deletion.");
                return NotFound("Unable to load user.");
            }

            var success = await _categoryService.DeleteCustomCategoryAsync(user.Id, CategoryId);

            if (success)
            {
                _logger.LogInformation("Category with ID {CategoryId} deleted successfully for user {UserId}.", CategoryId, user.Id);
                TempData["SuccessMessage"] = "Category deleted successfully.";
                return RedirectToPage("./UserCategories");
            }
            else
            {
                _logger.LogWarning("Failed to delete category with ID {CategoryId} for user {UserId}.", CategoryId, user.Id);
                ModelState.AddModelError(string.Empty, "Category not found or could not be deleted.");
                return Page();
            }
        }
    }
}
