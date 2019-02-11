using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Service.Shared
{
    public interface IPriceService
    {
        [Get("/api/v1/price?assetId={assetId}")]
        Task<IPrice> GetPrice(string assetId);
        [Get("/api/v1/price")]
        Task<IEnumerable<IPrice>> GetAllPrices();
        [Put("/api/v1/price")]
        Task<IPrice> CreatePrice(PriceCreationRequest request);
    }
}
