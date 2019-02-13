using AzurePlayground.Service.Shared;
using Dasein.Core.Lite;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace AzurePlayground.Service
{
    public class TradesController : ServiceControllerBase<ITradeService>
    {
        [Authorize]
        [HttpPut]
        [ProducesResponseType(typeof(TradeCreationResult), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<TradeCreationResult>> CreateTrade([FromBody] TradeCreationRequest request)
        {
            request.Trader = HttpContext.User.Identity.Name;

            var tradeResult = await Service.CreateTrade(request);
            return CreatedAtAction(nameof(GetTradeById), new { tradeId = tradeResult.TradeId }, tradeResult);
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ITrade>),(int)HttpStatusCode.OK)]
        public async Task<IEnumerable<ITrade>> GetAllTrades()
        {
            return await Service.GetAllTrades();
        }

        [Authorize]
        [HttpGet("{tradeId}")]
        [ProducesResponseType(typeof(ITrade), (int)HttpStatusCode.OK)]
        public async Task<ITrade> GetTradeById([FromRoute] Guid tradeId)
        {
            return await Service.GetTradeById(tradeId);
        }
    }
}
