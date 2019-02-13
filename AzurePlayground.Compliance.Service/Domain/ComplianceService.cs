using AzurePlayground.Events.EventStore;
using AzurePlayground.EventStore;
using AzurePlayground.EventStore.Infrastructure;
using AzurePlayground.Service.Shared;
using Dasein.Core.Lite.Shared;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzurePlayground.Service
{
    public class ComplianceService : IHostedService, ICanLog
    {
        private readonly IEventStoreCache<Guid, Trade, MutatedEntitiesDto<Trade>> _cache;
        private readonly IEventStoreRepository _repository;
        private readonly ComplianceServiceConfiguration _configuration;
        private IDisposable _cleanup;

        public ComplianceService(IEventStoreRepository repository, ComplianceServiceConfiguration configuration, IEventStoreCache<Guid, Trade, MutatedEntitiesDto<Trade>> cache)
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
                                if (obs.Trades.Count == 0 || obs.IsCacheState) return;

                                var relevantChanges = obs.Trades.Where(trade => trade.Status == TradeStatus.Filled);

                                foreach (var change in relevantChanges)
                                {
                                    Scheduler.Default.Schedule(async () =>
                                    {
                                        try
                                        {

                                            var trade = await _repository.GetById<Trade>(change.Id);

                                            if (TradeServiceReferential.Rand.Next(1, 10) == 1)
                                            {
                                                var tradeRejectedEvent = new ComplianceRejectTrade()
                                                {
                                                    EntityId = change.Id,
                                                    ComplianceService = _configuration.Id
                                                };

                                                trade.ApplyEvent(tradeRejectedEvent);
                                            }
                                            else
                                            {

                                                var counterparty = TradeServiceReferential.Counterparties.Random();
                                                var price = TradeServiceReferential.Assets.FirstOrDefault(asset => asset.Name == trade.Asset).Price;

                                                await Task.Delay(1000);

                                                var bookTradeEvent = new BookTrade()
                                                {
                                                    EntityId = change.Id,
                                                    ComplianceService = _configuration.Id
                                                };

                                                trade.ApplyEvent(bookTradeEvent);

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
