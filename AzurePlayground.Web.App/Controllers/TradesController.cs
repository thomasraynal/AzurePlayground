using AzurePlayground.Service.Shared;
using AzurePlayground.Web.App.Infrastructure;
using AzurePlayground.Web.App.Models;
using Dasein.Core.Lite.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AzurePlayground.Web.App
{
    public class TradesController : Controller
    {
        private ITradeService _tradeService;

        [Authorize]
        public async Task<IActionResult> Index()
        {
            try
            {
                var trades = await _tradeService.GetAllTrades();

                var vm = new TradesViewModel()
                {
                    Trades = trades.Select(trade => new TradeViewModel(trade))
                };

                return View("Trades", vm);

            }
            catch (HttpRequestException ex)
            {
                var vm = new ErrorViewModel();
                var message = ex.Message;
                return View("Error", vm);
            }
        }

        public TradesController(AppConfiguration configuration)
        {
            _tradeService = ApiServiceBuilder<ITradeService>.Build(configuration.Gateway)
                                                .AddAuthorizationHeader(() =>
                                                {
                                                    return HttpContext.GetTokenAsync("access_token").Result;
                                                })
                                                .Create();
        }
    }
}
