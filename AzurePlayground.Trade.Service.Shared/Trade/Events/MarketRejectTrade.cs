using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service.Shared
{
    public class MarketRejectTrade : EventBase<Guid, Trade>
    {
        public string MarketService { get; set; }
        public TradeStatus Status => TradeStatus.PreTradeCheckFailed;

        public override void Apply(Trade entity)
        {
            entity.Status = Status;
            entity.MarketService = MarketService;
        }
    }
}
