﻿using AutoMapper;
using Buddget.BLL.DTOs;
using Buddget.BLL.Services.Interfaces;
using BuddgetWeb.Areas.User.Models;
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
            int userId = 1; // PLUG
            var spaces = await _financialSpaceService.GetFinancialSpacesUserIsMemberOrOwnerOf(userId);

            var viewModel = new BuddgetWeb.Areas.User.Models.MySpacesViewModel
            {
                FinancialSpaces = spaces.ToList(),
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Index(int id)
        {
            _logger.LogInformation("Loading financial space with ID {SpaceId}", id);
            var space = await _financialSpaceService.GetFinancialSpaceByIdAsync(id);
            if (space == null)
            {
                _logger.LogWarning("Financial space with ID {SpaceId} not found", id);
                return View("NotFound");
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

        [HttpPost]
        public async Task<IActionResult> DeleteMember(int spaceId, int memberId)
        {
            var requestingUserId = int.Parse(User.FindFirst("UserId")?.Value ?? "1");

            try
            {
                _logger.LogInformation("Deleting member {MemberId} from space {SpaceId}", memberId, spaceId);
                var resultMessage = await _financialSpaceMemberService.DeleteMemberAsync(spaceId, memberId, requestingUserId);
                _logger.LogInformation("Member {MemberId} successfully deleted from space {SpaceId}", memberId, spaceId);

                TempData["Message"] = resultMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting member {MemberId} from space {SpaceId}", memberId, spaceId);
                TempData["Message"] = "An unexpected error occurred while trying to delete the member.";
            }

            return RedirectToAction(nameof(Index), new { id = spaceId });
        }


        [HttpPost]
        public async Task<IActionResult> InviteMember(string email, int spaceId)
        {
            try
            {
                _logger.LogInformation("Inviting user with email {Email} to space {SpaceId}", email, spaceId);
                await _financialSpaceMemberService.InviteMember(email, spaceId);
                _logger.LogInformation("User with email {Email} successfully invited to space {SpaceId}", email, spaceId);
                TempData["Message"] = "User successfully invited to the space.";
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Cannot invite user with email {Email} to space {SpaceId}", email, spaceId);
                TempData["Message"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inviting user with email {Email} to space {SpaceId}", email, spaceId);
                TempData["Message"] = "An error occurred while inviting the user.";
            }

            return RedirectToAction(nameof(Index), new { id = spaceId });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateFinancialSpaceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                _logger.LogError("Model binding errors: {Errors}", string.Join(", ", errors));

                _logger.LogInformation("Name: {Name}", model.Name);
                _logger.LogInformation("Description: {Description}", model.Description);
                return View(model);
            }

            try
            {
                var financialSpaceDto = new FinancialSpaceDto
                {
                    Name = model.Name,
                    Description = model.Description,
                    OwnerId = int.Parse(User.FindFirst("UserId")?.Value ?? "1")
                };

                if (model.Image != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.Image.CopyToAsync(memoryStream);
                        financialSpaceDto.ImageData = memoryStream.ToArray();
                        financialSpaceDto.ImageName = model.Image.FileName;
                    }
                }

                var resultMessage = await _financialSpaceService.CreateFinancialSpaceAsync(financialSpaceDto);
                TempData["Message"] = resultMessage;

                if (resultMessage.Contains("successfully", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction(nameof(MySpaces));
                }
                else
                {
                    ModelState.AddModelError("", resultMessage);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating financial space");
                ModelState.AddModelError("", "An error occurred while creating the financial space.");
                return View(model);
            }
        }
    }
}
