using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service.Shared
{
    public class MarketOrderResult
    {
        public Guid TradeId { get; set; }
        public TradeWay Way { get; set; }
        public String Marketplace { get; set; }
    }
}
