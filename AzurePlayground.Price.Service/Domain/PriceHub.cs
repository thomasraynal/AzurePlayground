using AzurePlayground.Service.Shared;
using Dasein.Core.Lite;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace AzurePlayground.Service.Domain
{
    [Authorize]
    public class PriceHub : HubBase<Price>
    {
        public PriceHub(IHubContextHolder<Price> context) : base(context)
        {
        }

        public override string Name => nameof(PriceHub);
    }
}
