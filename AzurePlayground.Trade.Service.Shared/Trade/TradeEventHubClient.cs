using AzurePlayground.Service.Shared;
using Dasein.Core.Lite.Shared;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service.Domain
{
    public class TradeEventHubClient : SignalRServiceClientBase<Trade, TradeEventRequest>
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


        public TradeEventHubClient(TradeEventRequest request, HttpTransportType transports, Action<HttpConnectionOptions> configureHttpConnection) : base(request, transports, configureHttpConnection)
        {
        }

        public override string HubName => TradeServiceReferential.TradeEventHub;

        public override Func<HubConnectionBuilder> ConnectionBuilderProvider => _builder;
    }
}
