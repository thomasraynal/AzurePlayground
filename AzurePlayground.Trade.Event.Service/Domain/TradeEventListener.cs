using AzurePlayground.Service.Infrastructure;
using AzurePlayground.Service.Shared;
using Dasein.Core.Lite;
using Dasein.Core.Lite.Shared;
using DynamicData;
using EventStore.Client.Lite;
using Microsoft.Extensions.Hosting;
using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;

namespace AzurePlayground.Service.Domain
{
    public class TradeEventListener : IHostedService
    {

        private IEventStoreCache<Guid, Trade> _cache;
        private IDisposable _cleanup;
        private ISignalRService<Trade, TradeEventRequest> _tradeEventHubService;

        public TradeEventListener(TradeEventServiceConfiguration configuration, IEventStoreCache<Guid, Trade> cache)
        {
            _cache = cache;

            var accessTokenRetrieverFactory = new AccessTokenRetrieverFactory();

            var tokenRetriever = accessTokenRetrieverFactory.GetToken(
                             "internal",
                             "idkfa",
                             configuration.Identity,
                             AzurePlaygroundConstants.Auth.ClientReferenceToken,
                             AzurePlaygroundConstants.Api.Trade,
                             configuration.Key
                         );

            _tradeEventHubService = SignalRServiceBuilder<Trade, TradeEventRequest>
                              .Create()
                              .Build(new TradeEventRequest((p) => true), (opts) =>
                              {
                                  opts.AccessTokenProvider = () => Task.FromResult(tokenRetriever());
                              });

            _tradeEventHubService.Connect(Scheduler.Default, 2000);

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cleanup = _cache
                        .AsObservableCache()
                        .Connect()
                        .WhereReasonsAreNot(ChangeReason.Remove)
                        .Subscribe(async changes =>
                        {
                            foreach (var trade in changes)
                            {
                                await _tradeEventHubService.Current.Proxy.RaiseChange(trade);
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
