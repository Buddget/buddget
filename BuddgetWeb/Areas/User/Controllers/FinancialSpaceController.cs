using AutoMapper;
using Buddget.BLL.Services.Implementation;
using Buddget.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace BuddgetWeb.Areas.User.Controllers
{
    [Area("User")]
    public class FinancialSpaceController : Controller
    {
        private readonly IFinancialSpaceService _financialSpaceService;
        private readonly IMapper _mapper;
        private readonly ILogger<FinancialSpaceController> _logger;

        public FinancialSpaceController(IFinancialSpaceService financialSpaceService, IMapper mapper, ILogger<FinancialSpaceController> logger)
        {
            _financialSpaceService = financialSpaceService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IActionResult> MySpaces()
        {
            // Assuming userId is 1 for now - you should get this from authentication
            int userId = 1;
            var spaces = await _financialSpaceService.GetFinancialSpacesUserIsMemberOrOwnerOf(userId);

            var viewModel = new BuddgetWeb.Areas.User.Models.MySpacesViewModel
            {
                FinancialSpaces = spaces.ToList()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Index(int id)
        {
            var space = await _financialSpaceService.GetFinancialSpaceByIdAsync(id);
            if (space == null)
            {
                return NotFound();
            }

            return View(space);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "1");
            var resultMessage = await _financialSpaceService.DeleteFinancialSpaceAsync(userId, id);

            TempData["Message"] = resultMessage;

            return RedirectToAction(nameof(MySpaces));
        }

    }
}
