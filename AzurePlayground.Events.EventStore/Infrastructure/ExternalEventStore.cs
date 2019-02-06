using System;
using AzurePlayground.EventStore.Connection;
using EventStore.ClientAPI;

namespace AzurePlayground.EventStore.Infrastructure
{
    public class ExternalEventStore : IEventStore
    {
        public ExternalEventStore(Uri uri)
        {
            Connection = EventStoreConnection.Create(EventStoreConnectionSettings.Default, uri);
        }

        public IEventStoreConnection Connection { get; }
    }
}