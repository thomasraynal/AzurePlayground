using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Service.Shared
{
    public interface ITradeService
    {
        [Put("/api/v1/trades")]
        Task<TradeCreationResult> CreateTrade(TradeCreationRequest request);
        [Get("/api/v1/trades")]
        Task<IEnumerable<ITrade>> GetAllTrades();
        [Get("/api/v1/trades/{tradeId}")]
        Task<ITrade> GetTradeById(Guid tradeId);
    }
}
