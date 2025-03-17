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

        public async Task<IActionResult> Index(int id = 2)
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
