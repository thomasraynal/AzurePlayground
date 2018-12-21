using AzurePlayground.Service.Shared;
using Dasein.Core.Lite;
using System.Threading.Tasks;

namespace AzurePlayground.Service.Domain
{
    public class PriceHub : HubBase<Price>
    {
        public PriceHub(IHubContextHolder<Price> context) : base(context)
        {
        }

        public async Task RaisePriceChanged(Price price)
        {
            await RaiseChange(price, TradeServiceReferential.OnPriceChanged);
        }

        public override string Name => nameof(PriceHub);
    }
}
