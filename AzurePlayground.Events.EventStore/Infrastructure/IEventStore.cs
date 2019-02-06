using EventStore.ClientAPI;

namespace AzurePlayground.EventStore.Infrastructure
{
    public interface IEventStore
    {
        IEventStoreConnection Connection { get; }
    }
}