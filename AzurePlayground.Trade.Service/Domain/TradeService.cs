using AzurePlayground.Events.EventStore;
using AzurePlayground.EventStore;
using AzurePlayground.EventStore.Infrastructure;
using AzurePlayground.Service.Shared;
using Dasein.Core.Lite;
using Dasein.Core.Lite.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Service
{
    public class TradeService : IService<ITradeService>, ITradeService, ICanLog
    {
        private IEventStoreRepository _repository;
        private TradeServiceConfiguration _configuration;
        private IEventStoreCache<Guid, Trade, MutatedEntitiesDto<Trade>> _cache;

        public TradeService(IEventStoreRepository repository, TradeServiceConfiguration configuration, IEventStoreCache<Guid, Trade, MutatedEntitiesDto<Trade>> cache)
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
                EntityId = trade.Id,
                Asset = request.Asset,
                Currency = request.Currency,
                Volume = request.Volume,
                Way = request.Way,
                TradeService = _configuration.Id
            };

            trade.ApplyEvent(tradeCreationEvent);

            await _repository.SaveAsync(trade);

            return new TradeCreationResult()
            {
                TradeId = trade.Id
            };

        }

        public Task<IEnumerable<ITrade>> GetAllTrades()
        {
            return Task.FromResult(_cache.CacheState.State.Select(trade => trade.Value).Cast<ITrade>());
        }

        public async Task<ITrade> GetTradeById(Guid tradeId)
        {
            return await _repository.GetById<Trade>(tradeId);
        }
    }
}
