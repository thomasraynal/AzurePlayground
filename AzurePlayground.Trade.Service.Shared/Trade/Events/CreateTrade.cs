using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service.Shared
{
    public class CreateTrade : EventBase<Guid,Trade>
    {
        public TradeStatus Status => TradeStatus.Created;
        public TradeWay Way { get; set; }
        public String Asset { get; set; }
        public double Volume { get; set; }
        public String Currency { get; set; }
        public String TradeService { get; set; }

        public override void Apply(Trade entity)
        {
            entity.Status = Status;
            entity.Id = EntityId;
            entity.Way = Way;
            entity.Asset = Asset;
            entity.Currency = Currency;
            entity.Volume = Volume;
            entity.TradeService = TradeService;
        }
    }
}
