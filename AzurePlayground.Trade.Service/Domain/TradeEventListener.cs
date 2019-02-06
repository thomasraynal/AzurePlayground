using AzurePlayground.Events.EventStore;
using AzurePlayground.EventStore;
using AzurePlayground.Service.Shared;
using Dasein.Core.Lite.Shared;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;

namespace AzurePlayground.Service
{
    public class TradeEventListener : IHostedService, ICanLog
    {
        private IEventStoreCache<Guid, Trade, MutatedEntitiesDto<Trade>> _cache;
        private IDisposable _cleanup;

        public TradeEventListener(IEventStoreCache<Guid, Trade, MutatedEntitiesDto<Trade>> cache)
        {
            _cache = cache;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cleanup = _cache
                        .GetOutputStream()
                        .Subscribe(obs =>
                        {
                            foreach (var trade in obs.Trades)
                            {
                                this.LogInformation($"Handle trade {trade}");
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
