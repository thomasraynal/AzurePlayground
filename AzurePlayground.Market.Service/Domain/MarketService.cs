using AzurePlayground.Events.EventStore;
using AzurePlayground.EventStore;
using AzurePlayground.EventStore.Infrastructure;
using AzurePlayground.Service.Shared;
using Dasein.Core.Lite.Shared;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;

namespace AzurePlayground.Service
{
    public class MarketService : ICanLog, IHostedService
    {
        private readonly IEventStoreCache<Guid, Trade, MutatedEntitiesDto<Trade>> _cache;
        private readonly IEventStoreRepository _repository;
        private readonly MarketServiceConfiguration _configuration;
        private IDisposable _cleanup;

        public MarketService(IEventStoreRepository repository, MarketServiceConfiguration configuration, IEventStoreCache<Guid, Trade, MutatedEntitiesDto<Trade>> cache)
        {
            _cache = cache;
            _repository = repository;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cleanup = _cache
                          .GetOutputStream()
                          .Subscribe(obs =>
                          {
                              if (obs.Trades.Count == 0) return;

                              var relevantChanges = obs.Trades.Where(trade => trade.Status == TradeStatus.Created);

                              foreach (var change in relevantChanges)
                              {
                                  Scheduler.Default.Schedule(async () =>
                                  {
                                      try
                                      {

                                          var trade = await _repository.GetById<Trade>(change.Id);

                                          if (TradeServiceReferential.Rand.Next(1, 10) == 1)
                                          {
                                              var tradeRejectedEvent = new MarketRejectTrade()
                                              {
                                                  EntityId = trade.Id,
                                                  MarketService = _configuration.Id,
                                              };

                                              trade.ApplyEvent(tradeRejectedEvent);
                                          }
                                          else
                                          {

                                              var counterparty = TradeServiceReferential.Counterparties.Random();
                                              var price = TradeServiceReferential.Assets.FirstOrDefault(asset => asset.Name == trade.Asset).Price;

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
                                                  EntityId = trade.Id,
                                                  Counterparty = counterparty,
                                                  Price = price,
                                                  Date = DateTime.Now,
                                                  MarketService = _configuration.Id,
                                                  Marketplace = TradeServiceReferential.Markets.Random().Name,
                                              };

                                              trade.ApplyEvent(fillTradeEvent);
                                          }

                                          await _repository.SaveAsync(trade);

                                          this.LogInformation($"Handle trade {trade}");
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
