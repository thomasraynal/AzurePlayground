using AzurePlayground.Service.Shared;
using AzurePlayground.Wpf.App.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Wpf.App
{
    public class TradeViewModel : ViewModelBase
    {
        private ITrade _trade;

        public TradeViewModel(ITrade trade)
        {
            _trade = trade;
            _currentPrice = -1;
        }

        public void ApplyChange(ITrade dto)
        {
            _trade = dto;

            this.OnPropertyChanged(nameof(Id));
            this.OnPropertyChanged(nameof(Status));
            this.OnPropertyChanged(nameof(Marketplace));
            this.OnPropertyChanged(nameof(Counterparty));
            this.OnPropertyChanged(nameof(Status));
            this.OnPropertyChanged(nameof(Way));
            this.OnPropertyChanged(nameof(Price));
            this.OnPropertyChanged(nameof(Currency));
            this.OnPropertyChanged(nameof(Volume));
            this.OnPropertyChanged(nameof(TradeService));
            this.OnPropertyChanged(nameof(Trader));
            this.OnPropertyChanged(nameof(ComplianceService));
        }

        private double _currentPrice;
        public double CurrentPrice
        {
            get
            {
                return _currentPrice;
            }
            set
            {
                _currentPrice = value;
                this.OnPropertyChanged(nameof(CurrentPrice));
            }
        }

        public TradeStatus Status
        {
            get
            {
                return _trade.Status;
            }

            set
            {
                _trade.Status = value;
                this.OnPropertyChanged(nameof(Status));
            }
        }

        public Guid Id => _trade.EntityId;
        public DateTime Date => _trade.Date;
        public string Marketplace => _trade.Marketplace;
        public string Counterparty => _trade.Counterparty;
        public string Asset => _trade.Asset;
        public TradeWay Way => _trade.Way;
        public double Price => _trade.Price;
        public string Currency => _trade.Currency;
        public double Volume => _trade.Volume;
        public string TradeService => _trade.TradeService;
        public string ComplianceService => _trade.ComplianceService;
        public string Trader => _trade.Trader;

    }

}
