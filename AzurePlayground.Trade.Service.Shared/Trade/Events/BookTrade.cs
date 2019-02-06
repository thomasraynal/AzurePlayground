using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service.Shared
{
    public class BookTrade : EventBase<Guid, Trade>
    {
        public string ComplianceService { get; set; }
        public TradeStatus Status => TradeStatus.Booked;

        public override void Apply(Trade entity)
        {
            entity.Status = Status;
            entity.ComplianceService = ComplianceService;
        }
    }
}
