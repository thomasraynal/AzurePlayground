using AzurePlayground.Service.Shared;
using Dasein.Core.Lite.Shared;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace AzurePlayground.Service.Domain
{
    public class PricePublisher : IPublisher, ICanLog
    {
        private Random _rand;
        private PriceHub _priceHub;
        private IPriceService _priceService;
        private List<IPrice> _priceHistory;
        private IDisposable _priceGenerator;
        private IServiceConfiguration _configuration;
        private CompositeDisposable _dispose;

        public PricePublisher(PriceHub priceHub, IPriceService priceService, IServiceConfiguration configuration)
        {
            _rand = new Random();

            _priceHub = priceHub;
            _priceService = priceService;
            _configuration = configuration;
        
            _dispose = new CompositeDisposable();
        }

        public async Task Start()
        {

            _priceHistory = new List<IPrice>(await _priceService.GetAllPrices());

            _priceGenerator = Observable
              .Interval(TimeSpan.FromMilliseconds(250))
              .Delay(TimeSpan.FromMilliseconds(50))
              .Subscribe(async _ =>
              {

                  var asset = TradeServiceReferential.Assets.Random();
                  var price = CreatePrice(asset);
                  await _priceService.CreatePrice(price);

                  _priceHistory.Add(price);
                  await _priceHub.RaisePriceChanged(price);

              });

            _dispose.Add(_priceGenerator);
        }

        private const double maxDeviation = 0.20;

        private bool IsMaxDeviationReached(String asset, double priceCandidate)
        {
            var first = _priceHistory.FirstOrDefault(price => price.Asset == asset);

            if (null == first) return false;

            return (Math.Abs(first.Value - priceCandidate) / first.Value) > maxDeviation;
        }

        private double GetPrice(String asset)
        {
            var way = _rand.Next(2) == 0 ? -1.0 : 1.0;
            var last = _priceHistory.LastOrDefault(price => price.Asset == asset);

            if(null == last)
            {
                return TradeServiceReferential.Assets.First(a => a.Name == asset).Price;
            }

            return last.Value + (way * last.Value * 0.05);
        }

        private Price CreatePrice(Asset asset)
        {

            var newPrice = GetPrice(asset.Name);

            while (IsMaxDeviationReached(asset.Name, newPrice))
            {
                newPrice = GetPrice(asset.Name);
            }

            return new Price(Guid.NewGuid(), asset.Name, newPrice, DateTime.Now);
        }

        public Task Stop()
        {
            _priceGenerator.Dispose();
            return Task.CompletedTask;
        }
    }
}
