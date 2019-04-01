using AzurePlayground.Service.Shared;
using Dasein.Core.Lite.Shared;
using EventStore.Client.Lite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzurePlayground.Service
{
    public class TradeService : IService<ITradeService>, ITradeService, ICanLog
    {
        private IEventStoreRepository<Guid> _repository;
        private TradeServiceConfiguration _configuration;
        private IEventStoreCache<Guid, Trade> _cache;
        
        public TradeService(IEventStoreRepository<Guid> repository, TradeServiceConfiguration configuration, IEventStoreCache<Guid, Trade> cache)
        {
            _repository = repository;
            _configuration = configuration;
            _cache = cache;
        }

        public async Task<TradeCreationResult> CreateTrade(TradeCreationRequest request)
        {
        
            var trade = new Trade();

            var tradeCreationEvent = new CreateTrade()
            {
                Asset = request.Asset,
                Currency = request.Currency,
                Volume = request.Volume,
                Way = request.Way,
                TradeService = _configuration.Id,
                Trader = request.Trader
            };

            await _repository.Apply(trade, tradeCreationEvent);

            return new TradeCreationResult()
            {
                TradeId = trade.EntityId
            };

        }

        public Task<IEnumerable<ITrade>> GetAllTrades()
        {
            return Task.FromResult(_cache.AsObservableCache().Items.Select(trade => trade).Cast<ITrade>());
        }

        public async Task<ITrade> GetTradeById(Guid tradeId)
        {
            return await _repository.GetById<Trade>(tradeId);
        }
    }
}
