using AzurePlayground.Service.Shared;
using Dasein.Core.Lite.Shared;
using EventStore.Client.Lite;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Refit;
using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;

namespace AzurePlayground.Service
{
    public class MarketService : ICanLog, IHostedService
    {
        private readonly IEventStoreCache<Guid, Trade,Trade> _cache;
        private readonly IEventStoreRepository<Guid> _repository;
        private readonly MarketServiceConfiguration _configuration;
        private IDisposable _cleanup;
        private IPriceService _priceService;

        public MarketService(IEventStoreRepository<Guid> repository, MarketServiceConfiguration configuration, IEventStoreCache<Guid, Trade, Trade> cache)
        {
            _cache = cache;
            _repository = repository;
            _configuration = configuration;

            var settings = AppCore.Instance.Get<JsonSerializerSettings>();

            var refitSettings = new RefitSettings()
            {
                JsonSerializerSettings = settings,
                HttpMessageHandlerFactory = () => new HttpRetryForeverMessageHandler(5000)
            };

            _priceService = ApiServiceBuilder<IPriceService>
                                    .Build(configuration.Gateway)
                                    .Create(refitSettings: refitSettings);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cleanup = _cache
                          .GetStream()
                          .Subscribe(obs =>
                          {
                              if (obs.Entities.Count == 0 || obs.IsCacheState) return;

                              var relevantChanges = obs.Entities.Where(trade => trade.Status == TradeStatus.Created);

                              foreach (var change in relevantChanges)   
                              {
                                  Scheduler.Default.Schedule(async () =>
                                  {
                                      try
                                      {
                                          var trade = await _repository.GetById<Trade>(change.EntityId);

                                          if (TradeServiceReferential.Rand.Next(1, 10) == 1)
                                          {
                                              var tradeRejectedEvent = new MarketRejectTrade()
                                              {
                                                  MarketService = _configuration.Id,
                                              };

                                              trade.ApplyEvent(tradeRejectedEvent);
                                          }
                                          else
                                          {

                                              var counterparty = TradeServiceReferential.Counterparties.Random();

                                              var price = await _priceService.GetPrice(change.Asset);

                                              if (counterparty == TradeServiceReferential.HighLatencyCounterparty)
                                              {
                                                  await Task.Delay(5000);
                                              }
                                              else
                                              {
                                                  await Task.Delay(1000);
                                              }

                                              var fillTradeEvent = new FillTrade()
                                              {
                                                  Counterparty = counterparty,
                                                  Price = price.Value,
                                                  Date = DateTime.Now,
                                                  MarketService = _configuration.Id,
                                                  Marketplace = TradeServiceReferential.Markets.Random().Name,
                                              };

                                              trade.ApplyEvent(fillTradeEvent);
                                          }

                                          await _repository.SaveAsync(trade);

                                      }
                                      catch (Exception ex)
                                      {
                                          this.LogInformation($"Exception while handling trade {change}", ex);
                                      }
              
                                  });
                              }
                          });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cleanup.Dispose();

            return Task.CompletedTask;
        }
        
    }
}
