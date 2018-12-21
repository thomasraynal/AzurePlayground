using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service.Shared
{
    public class Trade : ITrade
    {
        private Guid _id;
        private DateTime _date;
        private string _counterparty;
        private string _asset;
        private TradeStatus _status;
        private TradeWay _way;
        private double _price;
        private double _volume;

        public override string ToString()
        {
            return $"{Id} [{Counterparty}] [{Asset}]";
        }

        public override bool Equals(object obj)
        {
            return obj is ITrade && obj.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public Trade(Guid id, DateTime date, string counterparty, string asset, TradeStatus status, TradeWay way, double price, double volume)
        {
            _id = id;
            _date = date;
            _counterparty = counterparty;
            _asset = asset;
            _status = status;
            _way = way;
            _price = price;
            _volume = volume;
        }


        public Guid Id => _id;

        public DateTime Date => _date;

        public string Counterparty => _counterparty;

        public string Asset => _asset;

        public TradeStatus Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        public TradeWay Way => _way;

        public double PriceOnTransaction => _price;

        public double Volume => _volume;

    }
}
