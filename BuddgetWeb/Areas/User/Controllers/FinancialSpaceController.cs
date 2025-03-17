using AutoMapper;
using Buddget.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BuddgetWeb.Areas.User.Controllers
{
    [Area("User")]
    public class FinancialSpaceController : Controller
    {
        private readonly IFinancialSpaceService _financialSpaceService;
        private readonly IMapper _mapper;

        public FinancialSpaceController(IFinancialSpaceService financialSpaceService, IMapper mapper)
        {
            _financialSpaceService = financialSpaceService;
            _mapper = mapper;
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
    }
}
