using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service.Shared
{
    public class Trade : AggregateBase, ITrade
    {

        public Trade()
        {
            Id = Guid.NewGuid();
        }

        public override string ToString()
        {
            return $"{Id} [{Status}] [{Asset}] [{Way}] [{Currency}] [{Volume}]";
        }

        public override bool Equals(object obj)
        {
            return obj is ITrade && obj.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
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

        public override object Identifier => Id;

        public string MarketService { get; set; }
        public string TradeService { get; set; }
        public string ComplianceService { get; set; }

    }
}
