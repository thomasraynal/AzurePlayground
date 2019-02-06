using AzurePlayground.EventStore;
using AzurePlayground.EventStore.Infrastructure;
using AzurePlayground.Service.Shared;
using EventStore.ClientAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AzurePlayground.Events.EventStore
{
    public class TradeEventCache : EventStoreCache<Guid, Trade, MutatedEntitiesDto<Trade>>
    {

        public TradeEventCache(IObservable<IConnected<IEventStoreConnection>> eventStoreConnectionStream) : base(eventStoreConnectionStream)
        {
        }

        protected override MutatedEntitiesDto<Trade> CreateResponseFromCacheState(CacheState<Guid, Trade> container)
        {
            return new MutatedEntitiesDto<Trade>(container.State.Values.ToList(), true, container.IsStale);
        }

        protected override void UpdateCacheState(IDictionary<Guid, Trade> currentSotw, RecordedEvent evt)
        {

            var @event = evt.GetMutator<Guid,Trade>(EventTypes[evt.EventType]);

            if(null == @event)
            {
                throw new ArgumentOutOfRangeException("Unsupported event type");
            }

            if (Guid.Empty == @event.EntityId)
            {
                throw new MissingFieldException("Missing EntityId");
            }

            if (currentSotw.ContainsKey(@event.EntityId))
            {
                var current = currentSotw[@event.EntityId];
                current.ApplyEvent(@event, false);
            }
            else
            {
                var trade = new Trade();
                currentSotw.Add(@event.EntityId, trade);
                trade.ApplyEvent(@event, false);
            }

        }

        protected override MutatedEntitiesDto<Trade> MapSingleEventToUpdateDto(IDictionary<Guid, Trade> currentSotw, RecordedEvent evt)
        {
            var @event = evt.GetMutator<Guid,Trade>(EventTypes[evt.EventType]);

            if (null == @event)
            {
                throw new ArgumentOutOfRangeException("Unsupported event type");
            }

            if (Guid.Empty == @event.EntityId)
            {
                throw new MissingFieldException("Missing EntityId");
            }

            return CreateSingleEventUpdateDto(currentSotw, @event.EntityId);

        }

        protected override MutatedEntitiesDto<Trade> GetDisconnectedStaleUpdate()
        {
            return new MutatedEntitiesDto<Trade>(new List<Trade>(), false, true);
        }

        protected override bool IsValidUpdate(MutatedEntitiesDto<Trade> update)
        {
            return update != MutatedEntitiesDto<Trade>.Empty;
        }

        private static MutatedEntitiesDto<Trade> CreateSingleEventUpdateDto(IDictionary<Guid, Trade> currentSow, Guid member)
        {
            var trade = currentSow[member];

            return new MutatedEntitiesDto<Trade>(new[] { trade }, false, false);
        }
    }
}
