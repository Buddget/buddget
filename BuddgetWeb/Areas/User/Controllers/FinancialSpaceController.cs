﻿using AutoMapper;
using Buddget.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BuddgetWeb.Areas.User.Controllers
{
    [Area("User")]
    public class FinancialSpaceController : Controller
    {
        private readonly IFinancialSpaceService _financialSpaceService;
        private readonly IFinancialSpaceMemberService _financialSpaceMemberService;
        private readonly IMapper _mapper;
        private readonly ILogger<FinancialSpaceController> _logger;

        public FinancialSpaceController(IFinancialSpaceService financialSpaceService, IFinancialSpaceMemberService financialSpaceMemberService, IMapper mapper, ILogger<FinancialSpaceController> logger)
        {
            _financialSpaceService = financialSpaceService;
            _financialSpaceMemberService = financialSpaceMemberService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IActionResult> MySpaces()
        {
            int userId = 1;
            var spaces = await _financialSpaceService.GetFinancialSpacesUserIsMemberOrOwnerOf(userId);

            var viewModel = new BuddgetWeb.Areas.User.Models.MySpacesViewModel
            {
                FinancialSpaces = spaces.ToList(),
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Index(int id)
        {
            var space = await _financialSpaceService.GetFinancialSpaceByIdAsync(id);
            if (space == null)
            {
                return View("NotFound"); // Return a specific view for not found
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
        [HttpPost]
        public async Task<IActionResult> BanMember(int spaceId, int memberId)
        {   
            var requestingUserId = int.Parse(User.FindFirst("UserId")?.Value ?? "1");


            try
            {
                _logger.LogInformation("Banning member {MemberId} from space {SpaceId}", memberId, spaceId);
                var resultMessage = await _financialSpaceMemberService.BanMemberAsync(spaceId, memberId, requestingUserId);
                _logger.LogInformation("Member {MemberId} banned from space {SpaceId}", memberId, spaceId);
                TempData["Message"] = resultMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error banning member {MemberId} from space {SpaceId}", memberId, spaceId);
                TempData["Message"] = "An unexpected error occurred while trying to ban the member.";
            }

            return RedirectToAction(nameof(Index), new { id = spaceId });
        }
    }
}
