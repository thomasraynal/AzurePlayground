using AzurePlayground.Service.Shared;
using Dasein.Core.Lite.Shared;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service.Domain
{
    public class TradeEventHubClient : SignalRServiceClientBase<TradeEvent, TradeEventRequest>
    {
        private Func<HubConnectionBuilder> _builder;

        public TradeEventHubClient(TradeEventRequest request) : base(request)
        {
            _builder = () =>
            {
                return new HubConnectionBuilder().AddJsonProtocol(options =>
                {
                    options.PayloadSerializerSettings = AppCore.Instance.Get<JsonSerializerSettings>();
                });
            };
        }

        public override string HubName => TradeServiceReferential.TradeEventHub;

        public override string OnStreamUpdateMethodName => TradeServiceReferential.OnTradeEvent;

        public override Func<HubConnectionBuilder> ConnectionBuilderProvider => _builder;
    }
}
