using AzurePlayground.Service.Infrastructure;
using AzurePlayground.Service.Shared;
using Dasein.Core.Lite;
using Dasein.Core.Lite.Shared;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AzurePlayground.Service.Domain
{
    public class PricePublisher : IHostedService, ICanLog
    {
        private Random _rand;
        private IPriceService _priceService;
        private PriceServiceConfiguration _configuration;
        private CompositeDisposable _cleanUp;
        private IEnumerable<IPrice> _priceHistory;
        private ISignalRService<Price, PriceRequest> _priceHubService;

        public PricePublisher(IPriceService priceService, PriceServiceConfiguration configuration)
        {
            _rand = new Random();

            _priceService = priceService;
            _configuration = configuration;

            var accessTokenRetrieverFactory = new AccessTokenRetrieverFactory();

            var tokenRetriever = accessTokenRetrieverFactory.GetToken(
                             "internal",
                             "idkfa",
                             _configuration.Identity,
                             AzurePlaygroundConstants.Auth.ClientReferenceToken,
                             AzurePlaygroundConstants.Api.Trade,
                            _configuration.Key
                         );

            _priceHubService = SignalRServiceBuilder<Price, PriceRequest>
                              .Create()
                              .Build(new PriceRequest((p) => true), (opts) =>
                              {
                                  opts.AccessTokenProvider = () => Task.FromResult(tokenRetriever());
                              });

            _priceHubService.Connect(Scheduler.Default, 2000);

            _cleanUp = new CompositeDisposable();
        }

        public Task StartAsync(CancellationToken token)
        {

            var priceGenerator = Observable
              .Interval(TimeSpan.FromMilliseconds(250))
              .Delay(TimeSpan.FromMilliseconds(50))
              .Subscribe(async _ =>
              {

                  _priceHistory = await _priceService.GetAllPrices();

                  var asset = TradeServiceReferential.Assets.Random();
                  var request = CreatePriceRequest(asset);
                  var price = await _priceService.CreatePrice(request);

                  await _priceHubService.Current.Proxy.RaiseChange(price);

              });

            _cleanUp.Add(priceGenerator);

            return Task.CompletedTask;
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

            if (null == last)
            {
                return TradeServiceReferential.Assets.First(a => a.Name == asset).Price;
            }

            return last.Value + (way * last.Value * 0.05);
        }

        private PriceCreationRequest CreatePriceRequest(Asset asset)
        {

            var newPrice = GetPrice(asset.Name);

            while (IsMaxDeviationReached(asset.Name, newPrice))
            {
                newPrice = GetPrice(asset.Name);
            }

            return new PriceCreationRequest(asset.Name, newPrice, DateTime.Now);
        }

        public Task StopAsync(CancellationToken token)
        {
            _cleanUp.Dispose();
            return Task.CompletedTask;
        }


    }
}
