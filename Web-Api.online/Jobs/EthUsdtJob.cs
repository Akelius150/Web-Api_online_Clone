using Microsoft.AspNetCore.SignalR;
using Quartz;
using Web_Api.online.Data.Repositories;
using Web_Api.online.Hubs;
using Web_Api.online.Jobs.Abstract;

namespace Web_Api.online.Jobs
{
    [DisallowConcurrentExecution]
    public class EthUsdtJob : PairJobBase<EthUsdtHub>, IJob
    {
        public EthUsdtJob(IHubContext<EthUsdtHub> hubContext, TradeRepository tradeRepository)
            : base(hubContext, tradeRepository, "ETH_USDT")
        {
        }
    }
}