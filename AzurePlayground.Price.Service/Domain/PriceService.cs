using AzurePlayground.Service.Shared;
using Dasein.Core.Lite.Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Service.Domain
{
    public class PriceService : IService<IPriceService>, IPriceService, ICanLog
    {
        private List<Price> _prices;
        private object _locker = new object();

        public PriceService()
        {
            _prices = new List<Price>(TradeServiceReferential.Assets.Select(asset => new Price(Guid.NewGuid(), asset.Name, asset.Price, DateTime.Now)));
        }

        public Task<IPrice> CreatePrice(PriceCreationRequest request)
        {
            lock (_locker)
            {
                var price = new Price(Guid.NewGuid(), request.Asset, request.Value, request.Date);
                _prices.Add(price);
                return Task.FromResult(price as IPrice);
            }
        }

        public Task<IEnumerable<IPrice>> GetAllPrices()
        {
            return Task.FromResult(_prices.Cast<IPrice>().AsEnumerable());
        }

        public Task<IPrice> GetPrice(string asset)
        {
            return Task.FromResult(_prices.Cast<IPrice>().LastOrDefault(price => price.Asset == asset));
        }
    }
}
