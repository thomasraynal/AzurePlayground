using AzurePlayground.Service.Shared;
using System.Collections.Generic;

namespace AzurePlayground.Service.Shared
{

    public abstract class AggregateBase : IAggregate
    {
        private readonly List<IEvent> _pendingEvents = new List<IEvent>();

        public abstract object Identifier { get; }

        public int Version { get; private set; } = -1;

        public void Mutate<Tkey, TEntity>(IMutable<Tkey, TEntity> @event) where TEntity : AggregateBase
        {
            @event.Apply(this as TEntity);
            Version++;
        }

        public void ApplyEvent(IEvent @event, bool saveAsPendingEvent = true)
        {
            ((dynamic)this).Mutate((dynamic)@event);
            if (saveAsPendingEvent) _pendingEvents.Add(@event);
        }

        public ICollection<IEvent> GetPendingEvents()
        {
            return _pendingEvents;
        }

        public void ClearPendingEvents()
        {
            _pendingEvents.Clear();
        }

    }
}