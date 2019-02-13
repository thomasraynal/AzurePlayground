using Dasein.Core.Lite.Shared;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;

namespace AzurePlayground.Service.Shared
{
    public class PriceHubClient : SignalRServiceClientBase<Price, PriceRequest>
    {
        private Func<HubConnectionBuilder> _builder;

        protected override void Initialize()
        {
            _builder = () =>
            {
                return new HubConnectionBuilder().AddJsonProtocol(options =>
                {
                    options.PayloadSerializerSettings = AppCore.Instance.Get<JsonSerializerSettings>();
                });
            };
        }

        public PriceHubClient(PriceRequest request, HttpTransportType transports, Action<HttpConnectionOptions> configureHttpConnection) : base(request, transports, configureHttpConnection)
        {
        }

        public override string HubName => TradeServiceReferential.PriceHub;

        public override Func<HubConnectionBuilder> ConnectionBuilderProvider => _builder;
    }
}
