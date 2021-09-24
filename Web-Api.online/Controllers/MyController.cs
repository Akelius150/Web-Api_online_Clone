﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using Web_Api.online.Models.StoredProcedures;
using Web_Api.online.Models.Tables;
using Web_Api.online.Models.ViewModels;
using Web_Api.online.Repositories;

namespace Web_Api.online.Controllers
{
    public class MyController : Controller
    {
        private readonly EventsRepository _eventsRepository;
        private readonly UsersInfoRepository _usersInfoRepository;
        private readonly UserManager<IdentityUser> _usersManager;
        private readonly WalletsRepository _walletsRepository;
        private readonly TradeRepository _tradeRepository;
        public MyController(
            EventsRepository eventsRepository,
            UsersInfoRepository usersInfoRepository,
            UserManager<IdentityUser> usersManager,
            WalletsRepository walletsRepository,
            TradeRepository tradeRepository)
        {
            _eventsRepository = eventsRepository;
            _usersInfoRepository = usersInfoRepository;
            _usersManager = usersManager;
            _walletsRepository = walletsRepository;
            _tradeRepository = tradeRepository;
        }

        [HttpPost]
        public async Task<ActionResult> UpdateProfileData(ProfileViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Redirect("/Identity/Account/Login%2FMy%2FProfile");
            }

            model.UserInfo.UserId = userId;

            await _usersInfoRepository.spCreateOrUpdateProfileUserInfo(model.UserInfo);

            return RedirectToAction("Profile");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userId))
            {
                return Redirect("/Identity/Account/Login%2FMy%2FProfile");
            }

            UserInfoTableModel userInfo = (await _usersInfoRepository.spGetUserInfo_ByUserId(userId)) ?? new UserInfoTableModel();
            List<EventTableModel> lastThreeEvents = await _eventsRepository.spGetLastThreeEvents_ByUserId(userId);
            List<spGetNotEmptyValueWallet_ByUserId> notEmptyWallets = await _walletsRepository.GetNotEmptyWalletsByUserId(userId);
            decimal btcPrice = await _tradeRepository.spGetLastPrice_BTC_USDT_ClosedOrder();

            var user = await _usersManager.FindByIdAsync(userId);            

            var model = new ProfileViewModel()
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserInfo = userInfo,
                LastThreeEvents = lastThreeEvents,
                NotEmptyWallets = notEmptyWallets,
                BtcPrice = btcPrice
            };

            return View(model);
        }

        public async Task<IActionResult> Events()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Redirect("/Identity/Account/Login%2FMy%2FEvents");
            }

            return View(await _eventsRepository.GetByUserId(userId));
        }
    }
}
