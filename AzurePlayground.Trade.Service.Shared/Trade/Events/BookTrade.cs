using System;
using System.Collections.Generic;
using System.Text;
using EventStore.Client.Lite;

namespace AzurePlayground.Service.Shared
{
    public class BookTrade : EventBase<Guid, Trade>
    {
        public string ComplianceService { get; set; }
        public TradeStatus Status => TradeStatus.Booked;

        protected override void ApplyInternal(Trade entity)
        {
            entity.Status = Status;
            entity.ComplianceService = ComplianceService;
        }
    }
}
