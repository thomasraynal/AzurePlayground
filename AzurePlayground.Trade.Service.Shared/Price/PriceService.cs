using Dasein.Core.Lite.Shared;
using Dasein.Core.Lite.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Service.Shared
{
    public class PriceService : IPriceService
    {
        private List<IPrice> _prices;
        private ITradeService _tradeService;

        public PriceService(ITradeService tradeService)
        {
            _prices = Enumerable.Range(0, 600)
                 .Select(_ => new Price(Guid.NewGuid(), TradeServiceReferential.Assets.Random().Name, TradeServiceReferential.Rand.NextDouble(), DateTime.Now.AddDays(-1)))
                 .GroupBy(price => price.Asset)
                 .Where(group => group.Count() == 1)
                 .SelectMany(group => group)
                 .Cast<IPrice>()
                 .ToList();

            _tradeService = tradeService;
        }

        public Task<IEnumerable<IPrice>> GetAllPrices()
        {
            //for in memory cache access...
            return Task.FromResult(_prices.Select(_ => _).ToList().AsEnumerable());
        }

        public Task<IEnumerable<IPrice>> GetPricesNoCache()
        {
            return Task.FromResult(_prices.Select(_=>_).ToList().AsEnumerable());
        }

        public Task<IEnumerable<IPrice>> GetPricesByAsset(string asset)
        {
            return Task.FromResult(_prices.Where((price) => price.Asset == asset));
        }

        public Task CreatePrice(IPrice price)
        {
            _prices.Add(price);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<IPrice>> GetPricesByTrade(Guid tradeId)
        {
            var trade = await _tradeService.GetTradeById(tradeId);

            if (null == trade) return Enumerable.Empty<IPrice>();

            return _prices.Where(p => p.Asset == trade.Asset);
        }
    }
}
