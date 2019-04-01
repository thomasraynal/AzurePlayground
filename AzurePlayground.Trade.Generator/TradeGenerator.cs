using AzurePlayground.Service.Shared;
using Dasein.Core.Lite.Shared;
using DynamicData;
using EventStore.Client.Lite;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Refit;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AzurePlayground.Generator
{
    public class TradeGenerator : IHostedService, ICanLog
    {
        private ITradeService _tradeService;
        private TradeGeneratorConfiguration _configuration;
        private IEventStoreCache<Guid, Trade> _cache;
        private IEventStoreRepository<Guid> _repository;
        private JwtSecurityTokenHandler _jwthandler;
        private CompositeDisposable _cleanup;

        public TradeGenerator(TradeGeneratorConfiguration configuration, IEventStoreRepository<Guid> repository, IEventStoreCache<Guid, Trade> cache)
        {
            _configuration = configuration;

            _cache = cache;
            _repository = repository;
            _jwthandler = new JwtSecurityTokenHandler();
            _cleanup = new CompositeDisposable();

            var settings = AppCore.Instance.Get<JsonSerializerSettings>();
            
            var refitSettings = new RefitSettings()
            {
                ContentSerializer = new JsonContentSerializer(settings),
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
                                .AsObservableCache()
                                .Connect()
                                .WhereReasonsAreNot(ChangeReason.Remove)
                                .Subscribe(changes =>
                                {
                                    foreach (var change in changes)
                                    {
                                        this.LogInformation($"Handle trade {change.Current}");
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
