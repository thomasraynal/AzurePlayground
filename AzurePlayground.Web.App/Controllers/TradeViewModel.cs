using AzurePlayground.Service.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzurePlayground.Web.App
{
    public class TradeViewModel: ITrade
    {
        public TradeViewModel(ITrade trade)
        {
            Id = trade.Id;
            Date = trade.Date;
            Counterparty = trade.Counterparty;
            Asset = trade.Asset;
            Currency = trade.Currency;
            Marketplace = trade.Marketplace;
            Status = trade.Status;
            Way = trade.Way;
            Price = trade.Price;
            Volume = trade.Volume;
            Trader = trade.Trader;
            TradeService = trade.TradeService;
            ComplianceService = trade.ComplianceService;

        }

        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public string Counterparty { get; set; }

        public string Asset { get; set; }

        public string Currency { get; set; }

        public string Marketplace { get; set; }

        public TradeStatus Status { get; set; }

        public TradeWay Way { get; set; }

        public double Price { get; set; }

        public double Volume { get; set; }

        public string Trader { get; set; }

        public string TradeService { get; set; }
        public string ComplianceService { get; set; }
    }
}
