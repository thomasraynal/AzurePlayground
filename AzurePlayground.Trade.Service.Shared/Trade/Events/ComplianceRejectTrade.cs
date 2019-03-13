using EventStore.Client.Lite;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service.Shared
{
    public class ComplianceRejectTrade : EventBase<Guid, Trade>
    {
        public string ComplianceService { get; set; }
        public TradeStatus Status => TradeStatus.ComplianceCheckFailed;

        protected override void ApplyInternal(Trade entity)
        {
            entity.Status = Status;
            entity.ComplianceService = ComplianceService;
        }
    }
}
