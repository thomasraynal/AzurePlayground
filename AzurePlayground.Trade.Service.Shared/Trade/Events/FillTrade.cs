using EventStore.Client.Lite;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service.Shared
{
    public class FillTrade : EventBase<Guid, Trade>
    {

        public TradeStatus Status => TradeStatus.Filled;
        public String Marketplace { get; set; }
        public double Price { get; set; }
        public String Counterparty { get; set; }
        public string MarketService { get; set; }
        public DateTime Date{ get; set; }

        protected override void ApplyInternal(Trade entity)
        {
            entity.Status = Status;
            entity.Marketplace = Marketplace;
            entity.Price = Price;
            entity.Date = Date;
            entity.Counterparty = Counterparty;
            entity.MarketService = MarketService;
        }
    }
}
