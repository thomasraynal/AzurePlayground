using AzurePlayground.Service.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service.Shared
{
    public class TradeEvent
    {
        public Guid TradeId { get; set; }

        public TradeStatus Status { get; set; }
    }
}
