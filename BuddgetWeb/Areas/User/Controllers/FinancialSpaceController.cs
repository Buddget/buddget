using AutoMapper;
using Buddget.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BuddgetWeb.Areas.User.Controllers
{
    [Area("User")]
    public class FinancialSpaceController : Controller
    {
        private readonly IFinancialSpaceService _financialSpaceService;
        private readonly IFinancialSpaceMemberService _financialSpaceMemberService;
        private readonly IMapper _mapper;
        private readonly ILogger<FinancialSpaceController> _logger;

        public FinancialSpaceController(
            IFinancialSpaceService financialSpaceService,
            IFinancialSpaceMemberService financialSpaceMemberService,
            IMapper mapper,
            ILogger<FinancialSpaceController> logger)
        {
            _financialSpaceService = financialSpaceService;
            _financialSpaceMemberService = financialSpaceMemberService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int id = 2)
        {
            _logger.LogInformation("Getting space info");
            var space = await _financialSpaceService.GetFinancialSpaceByIdAsync(id);
            if (space == null)
            {
                _logger.LogWarning("Financial space with ID {Id} not found.", id);
                return NotFound();
            }

            return View(space);
        }

        [HttpPost]
        public async Task<IActionResult> Index(InviteUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", new { id = model.SpaceId });
            }
            _logger.LogInformation(
                "InviteUser action called with email: {Email}, spaceId: {SpaceId}", 
                model.UserEmail ?? "null", model.SpaceId);
            if (string.IsNullOrEmpty(model.UserEmail))
            {
                _logger.LogWarning("Attempted to invite user with an empty email.");
                ModelState.AddModelError("", "Email cannot be empty.");
                return RedirectToAction("Index", new { id = model.SpaceId });
            }

            try
            {
                _logger.LogInformation("Inviting user with email {Email} to space ID {SpaceId}.", model.UserEmail, model.SpaceId);
                await _financialSpaceMemberService.InviteMember(model.UserEmail, model.SpaceId);

                TempData["SuccessMessage"] = "User has been invited successfully!";
                _logger.LogInformation("User with email {Email} invited successfully to space ID {SpaceId}.", model.UserEmail, model.SpaceId);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                _logger.LogError(ex, "Error occurred while inviting user {Email} to space ID {SpaceId}.", model.UserEmail, model.SpaceId);
            }

            return RedirectToAction("Index", new { id = model.SpaceId });
        }
    }
}
