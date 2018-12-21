using Dasein.Core.Lite.Shared;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Service.Shared
{
    public interface IPriceService
    {
        [Cached(500)]
        [Get("/api/v1/price")]
        Task<IEnumerable<IPrice>> GetAllPrices();

        Task CreatePrice(IPrice price);

        Task<IEnumerable<IPrice>> GetPricesNoCache();
        Task<IEnumerable<IPrice>> GetPricesByAsset(string asset);
        Task<IEnumerable<IPrice>> GetPricesByTrade(Guid tradeId);
    }
}
