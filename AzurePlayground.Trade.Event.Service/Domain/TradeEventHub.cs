using AzurePlayground.Service.Shared;
using Dasein.Core.Lite;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Service.Domain
{

    [Authorize]
    public class TradeEventHub : HubBase<Trade>
    {
        public TradeEventHub(IHubContextHolder<Trade> context) : base(context)
        {
        }

        public override string Name => nameof(TradeEventHub);

    }
}
