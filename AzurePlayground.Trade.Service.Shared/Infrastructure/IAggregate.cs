using AzurePlayground.Service.Shared;
using System.Collections.Generic;

namespace AzurePlayground.Service.Shared
{
    public interface IAggregate
    {
        int Version { get; }
        object Identifier { get; }
        void ApplyEvent(IEvent @event, bool saveAsPendingEvent);
        ICollection<IEvent> GetPendingEvents();
        void ClearPendingEvents();
    }
}