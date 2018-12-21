using AzurePlayground.Service.Shared;
using Dasein.Core.Lite;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzurePlayground.Service.Domain
{
    public class PriceController : ServiceControllerBase
    {
        private IPriceService _priceService;

        public PriceController(IPriceService priceService, JsonSerializerSettings settings)
        {
            _priceService = priceService;
        }

        [Authorize(TradeServiceReferential.TraderUserPolicy)]
        [HttpGet]
        public async Task<IEnumerable<IPrice>> GetPrices(bool cache = true)
        {
            if (cache) return await _priceService.GetAllPrices();

            return await _priceService.GetPricesNoCache();
        }
    }
}
