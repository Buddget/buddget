using AutoMapper;
using Buddget.BLL.DTOs;
using Buddget.BLL.Services.Interfaces;
using Buddget.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BuddgetWeb.Areas.Identity.Pages.Account.Manage
{
    public class UserCategoriesModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        private readonly UserManager<UserEntity> _userManager;
        private readonly ILogger<UserCategoriesModel> _logger;
        private readonly IMapper _mapper;

        public UserCategoriesModel(
            UserManager<UserEntity> userManager,
            ILogger<UserCategoriesModel> logger,
            ICategoryService categoryService,
            IMapper mapper)
        {
            _categoryService = categoryService;
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
        }

        public List<CategoryDto> UserCategories { get; set; } = new();

        [BindProperty]
        [Required(ErrorMessage = "Category name is required")]
        public string NewCategoryName { get; set; }

        public bool ShowNameRequiredModal { get; set; } = false;
        public bool ShowAlreadyExistsModal { get; set; } = false;

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadCategories();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(NewCategoryName))
                {
                    ShowNameRequiredModal = true;
                }

                await LoadCategories();
                return Page();
            }

            var success = await _categoryService.AddCustomCategoryAsync(user.Id, NewCategoryName);
            if (!success)
            {
                ShowAlreadyExistsModal = true;
                await LoadCategories();
                return Page();
            }

            return RedirectToPage(); // success
        }

        private async Task LoadCategories()
        {
            var user = await _userManager.GetUserAsync(User);
            var categories = await _categoryService.GetCustomCategoriesByUserIdAsync(user.Id);
            UserCategories = _mapper.Map<List<CategoryDto>>(categories);
        }
    }
}
