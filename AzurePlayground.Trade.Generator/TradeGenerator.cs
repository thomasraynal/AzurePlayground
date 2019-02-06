using AzurePlayground.Events.EventStore;
using AzurePlayground.EventStore;
using AzurePlayground.EventStore.Infrastructure;
using AzurePlayground.Service.Shared;
using Dasein.Core.Lite.Shared;
using IdentityModel.Client;
using IdentityServer4.Models;
using Microsoft.Extensions.Hosting;
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
        private string _token;
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

            _tradeService = ApiServiceBuilder<ITradeService>
                                    .Build(configuration.Gateway)
                                    .AddAuthorizationHeader(() =>
                                    {

                                        var jwttoken = _jwthandler.ReadToken(_token);

                                        if (jwttoken.ValidTo <= DateTime.Now)
                                        {
                                            _token = GetAccessToken().Result;
                                        }

                                        return _token;
                                    })
                                    .Create();

            _token = GetAccessToken().Result;


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
                                        catch { }

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

        private async Task<string> GetAccessToken()
        {
            var client = new HttpClient();

            var request = new DiscoveryDocumentRequest()
            {
                Policy =
                        {
                            RequireHttps = false,
                            RequireKeySet = false,
                            ValidateEndpoints = false,
                            ValidateIssuerName = false,
                        },
                Address = _configuration.Identity
            };

            var disco = await client.GetDiscoveryDocumentAsync(request);

            if (disco.IsError) throw new Exception(disco.Error);

            var accessToken = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = AzurePlaygroundConstants.Auth.ClientReferenceToken,
                ClientSecret = _configuration.Key,
                Scope = AzurePlaygroundConstants.Api.Trade,
                UserName = "bob.woodworth",
                Password = "bob"
            });


            if (accessToken.IsError)
            {
                throw new Exception("Failed to acquire token", accessToken.Exception);
            }

            return accessToken.AccessToken;
        }
    }
}
