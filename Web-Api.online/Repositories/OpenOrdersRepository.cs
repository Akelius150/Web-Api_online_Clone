﻿using Dapper;

using Microsoft.Extensions.Configuration;

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Web_Api.online.Data;
using Web_Api.online.Models;
using Web_Api.online.Models.Tables;
using Web_Api.online.Repositories.Abstract;

namespace Web_Api.online.Repositories
{
    public class OpenOrdersRepository
        : BaseRepository<BTC_USDT_OpenOrders, ExchangeContext>,
        IOpenOrdersRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ExchangeContext _context;
        public OpenOrdersRepository(
            ExchangeContext context,
            IConfiguration configuration)
            : base(context)
        {
            _context = context;
            _configuration = configuration;
        }

        public List<BTC_USDT_OpenOrders> GetByUserId(string userId)
        {
            return _context.BTC_USDT_OpenOrders
                .Where(x => x.CreateUserId == userId)
                .ToList();
        }

        public async Task<List<BTC_USDT_OpenOrders>> Get_BTC_USDT_OpenOrdersAsync()
        {
            using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("ExchangeConnection")))
            {
                List<BTC_USDT_OpenOrders> result = (List<BTC_USDT_OpenOrders>)
                    await db.QueryAsync<BTC_USDT_OpenOrders>("exec spGet_BTC_USDT_OpenOrders");

                return result;
            }
        }

        public async Task<List<OrderBookModel>> Get_BTC_USDT_OrderBookAsync(bool isBuy, int count = 15)
        {
            using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("ExchangeConnection")))
            {
                try
                {
                    var isBuyStr = isBuy ? "Buy" : "Sell";

                    var res = (await db.QueryAsync<OrderBookModel>($"spGet_BTC_USDT_SortedOrderBook{isBuyStr}", commandType: CommandType.StoredProcedure))
                        .Take(count);

                    return res.ToList();
                }
                catch { return null; }
            }
        }
    }
}
