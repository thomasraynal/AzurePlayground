using AzurePlayground.Events.EventStore;
using AzurePlayground.EventStore;
using AzurePlayground.EventStore.Infrastructure;
using AzurePlayground.Service.Shared;
using Dasein.Core.Lite.Shared;
using IdentityModel.Client;
using IdentityServer4.Models;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Polly;
using Refit;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzurePlayground.Generator
{
    public class TradeGenerator : IHostedService, ICanLog
    {
        private ITradeService _tradeService;
        private TradeGeneratorConfiguration _configuration;
        private IEventStoreCache<Guid, Trade, MutatedEntitiesDto<Trade>> _cache;
        private IEventStoreRepository _repository;
        private JwtSecurityTokenHandler _jwthandler;
        private CompositeDisposable _cleanup;

        public TradeGenerator(TradeGeneratorConfiguration configuration, IEventStoreRepository repository, IEventStoreCache<Guid, Trade, MutatedEntitiesDto<Trade>> cache)
        {
            _configuration = configuration;

            _cache = cache;
            _repository = repository;
            _jwthandler = new JwtSecurityTokenHandler();
            _cleanup = new CompositeDisposable();

            var settings = AppCore.Instance.Get<JsonSerializerSettings>();
            
            var refitSettings = new RefitSettings()
            {
                JsonSerializerSettings = settings,
                HttpMessageHandlerFactory = () => new HttpRetryForeverMessageHandler(5000)
            };

            var accessTokenRetrieverFactory = new AccessTokenRetrieverFactory();

            _tradeService = ApiServiceBuilder<ITradeService>
                                    .Build(configuration.Gateway)
                                    .AddAuthorizationHeader(accessTokenRetrieverFactory.GetToken(
                                        "bob.woodworth",
                                        "bob",
                                        _configuration.Identity,
                                        AzurePlaygroundConstants.Auth.ClientReferenceToken,
                                        AzurePlaygroundConstants.Api.Trade,
                                       _configuration.Key
                                      
                                    ))
                                    .Create(refitSettings: refitSettings);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var generationDisposable = Observable.Interval(TimeSpan.FromSeconds(1))
                                    .Subscribe(async _ =>
                                    {
                                        try
                                        {

                                            var requestResult = await _tradeService.CreateTrade(new TradeCreationRequest()
                                            {
                                                Asset = TradeServiceReferential.Assets.Random().Name,
                                                Currency = "EUR",
                                                Volume = TradeServiceReferential.Rand.Next(10, 50),
                                                Way = TradeWay.Buy
                                            });

                                            this.LogInformation($"Trade Request - {requestResult.TradeId}");

                                        }
                                        catch( Exception ex) {
                                            this.LogError(ex);
                                        }

                                    });


            var observerDisposable = _cache
                                .GetOutputStream()
                                .Subscribe(obs =>
                                {

                                    if (obs.IsCacheState) return;

                                    foreach (var trade in obs.Trades)
                                    {
                                        this.LogInformation($"Handle trade {trade}");
                                    }

                                });


            _cleanup.Add(generationDisposable);
            _cleanup.Add(observerDisposable);


            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cleanup.Dispose();
            return Task.CompletedTask;
        }

    }
}
