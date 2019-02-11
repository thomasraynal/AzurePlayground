using AzurePlayground.Service.Shared;
using Dasein.Core.Lite;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace AzurePlayground.Service
{
    public class PriceController : ServiceControllerBase<IPriceService>
    {
        private IPriceService _priceService;

        public PriceController(IPriceService priceService, JsonSerializerSettings settings)
        {
            _priceService = priceService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IPrice), (int)HttpStatusCode.OK)]
        public async Task<IPrice> GetPrice([FromQuery] string assetId)
        {
            return await Service.GetPrice(assetId);
        }
    }
}
