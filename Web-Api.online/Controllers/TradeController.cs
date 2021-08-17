﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Api.online.Repositories;
using Web_Api.online.Models;
using System.Security.Claims;
using Web_Api.online.Repositories.Abstract;
using Web_Api.online.Models.Tables;
using Web_Api.online.Models.Enums;

namespace Web_Api.online.Controllers
{
    public class TradeController : Controller
    {
        private readonly WalletsRepository _walletsRepository;
        private readonly IOpenOrdersRepository _openOrdersRepository;
        private readonly IClosedOrdersRepository _closedOrdersRepository;

        public TradeController(
            WalletsRepository walletsRepository,
            IOpenOrdersRepository openOrdersRepository,
            IClosedOrdersRepository closedOrdersRepository)
        {
            _walletsRepository = walletsRepository;
            _openOrdersRepository = openOrdersRepository;
            _closedOrdersRepository = closedOrdersRepository;
        }

        // GET: TradeController
        public ActionResult Index()
        {
            return View();
        }

        public class BTC_USDTModel
        {
            public string UserId { get; set; }
            public List<Wallet> UserWallets { get; set; }
            public Wallet BtcWallet { get; set; }

            public Wallet UsdtWallet { get; set; }
            public List<MarketTradesModel> MarketTrades { get; set; }
            public List<BTC_USDT_OpenOrders> UserOpenOrders { get; set; }
            public List<OrderBookModel> BuyOrderBook { get; set; }
            public List<OrderBookModel> SellOrderBook { get; set; }
        }

        public async Task<ActionResult> CancelOrder(long id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("You are not authorized.");
            }

            var order = await _openOrdersRepository.FindByIdAsync(id);

            if (order == null)
            {
                return BadRequest("Order with this id doesn't exist.");
            }

            if (order.CreateUserId != userId)
            {
                return BadRequest("This is not your order.");
            }

            await _openOrdersRepository.RemoveAsync(order);
            await _closedOrdersRepository.CreateAsync(new BTC_USDT_ClosedOrders()
            {
                Amount = order.Amount,
                BoughtUserId = null,
                Status = Convert.ToBoolean(ClosedOrderStatus.Canceled),
                ClosedDate = DateTime.Now,
                ClosedOrderId = order.OpenOrderId,
                CreateDate = order.CreateDate,
                CreateUserId = order.CreateUserId,
                IsBuy = order.IsBuy,
                Price = order.Price,
                Total = order.Total
            });

            return Ok();
        }

        public async Task<ActionResult> OpenOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userId))
            {
                return View(_openOrdersRepository
                    .GetByUserId("53cd122d-6253-4981-b290-11471f67c528"));
            }

            return BadRequest("You're not authorized");
        }

        public async Task<ActionResult> ClosedOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userId))
            {
                return View(_closedOrdersRepository
                    .GetByUserId("53cd122d-6253-4981-b290-11471f67c528"));
            }

            return BadRequest("You're not authorized");
        }

        public async Task<ActionResult> BTCUSDT()
        {
            List<Wallet> userWallets = new List<Wallet>();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            BTC_USDTModel model = new BTC_USDTModel();

            if (!string.IsNullOrEmpty(userId))
            {
                model.UserId = userId;

                userWallets = await _walletsRepository.GetUserWalletsAsync(userId);

                model.UserWallets = userWallets;

                Wallet btcWallet = userWallets.FirstOrDefault(x => x.CurrencyAcronim == "BTC");

                if (btcWallet == null)
                {
                    btcWallet = new Wallet
                    {
                        UserId = userId,
                        CurrencyAcronim = "BTC",
                        Address = System.Guid.NewGuid().ToString().Replace("-", ""),
                        Value = 0
                    };

                    btcWallet = await _walletsRepository.CreateUserWalletAsync(btcWallet);
                }

                model.UserWallets.Add(btcWallet);

                model.BtcWallet = btcWallet;

                Wallet usdtWallet = userWallets.FirstOrDefault(x => x.CurrencyAcronim == "USDT");

                if (usdtWallet == null)
                {
                    usdtWallet = new Wallet
                    {
                        UserId = userId,
                        CurrencyAcronim = "USDT",
                        Address = System.Guid.NewGuid().ToString().Replace("-", ""),
                        Value = 0,
                        Created = DateTime.Now,
                        LastUpdate = DateTime.Now
                    };

                    usdtWallet = await _walletsRepository.CreateUserWalletAsync(usdtWallet);
                }

                model.UserWallets.Add(usdtWallet);

                model.UsdtWallet = usdtWallet;

                model.UserOpenOrders = _openOrdersRepository.GetByUserId(userId);
            }

            model.BuyOrderBook = await _openOrdersRepository.Get_BTC_USDT_OrderBookAsync(true);
            model.SellOrderBook = await _openOrdersRepository.Get_BTC_USDT_OrderBookAsync(false);
            model.MarketTrades = await _closedOrdersRepository.Get_BTC_USDT_ClosedOrders();

            return View(model);
        }

        // GET: TradeController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TradeController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TradeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TradeController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TradeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TradeController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TradeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
