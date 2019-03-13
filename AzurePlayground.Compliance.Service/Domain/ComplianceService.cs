using AzurePlayground.Service.Shared;
using Dasein.Core.Lite.Shared;
using EventStore.Client.Lite;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;

namespace AzurePlayground.Service
{
    public class ComplianceService : IHostedService, ICanLog
    {
        private readonly IEventStoreCache<Guid, Trade, Trade> _cache;
        private readonly IEventStoreRepository<Guid> _repository;
        private readonly ComplianceServiceConfiguration _configuration;
        private IDisposable _cleanup;

        public ComplianceService(IEventStoreRepository<Guid> repository, ComplianceServiceConfiguration configuration, IEventStoreCache<Guid, Trade,Trade> cache)
        {
            _cache = cache;
            _repository = repository;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cleanup = _cache
                            .GetStream()
                            .Subscribe(obs =>
                            {
                                if (obs.Entities.Count == 0 || obs.IsCacheState) return;

                                var relevantChanges = obs.Entities.Where(trade => trade.Status == TradeStatus.Filled);

                                foreach (var change in relevantChanges)
                                {
                                    Scheduler.Default.Schedule(async () =>
                                    {
                                        try
                                        {

                                            var trade = await _repository.GetById<Trade>(change.EntityId);

                                            if (TradeServiceReferential.Rand.Next(1, 10) == 1)
                                            {
                                                var tradeRejectedEvent = new ComplianceRejectTrade()
                                                {
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
