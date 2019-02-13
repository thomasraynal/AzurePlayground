using Dasein.Core.Lite.Shared;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AzurePlayground.Service.Shared
{
    public class TradeEventRequest : HubRequestBase<Trade>
    {
        public TradeEventRequest(Expression<Func<Trade, bool>> filter) : base(filter)
        {
        }
    }
}
