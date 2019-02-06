using System;
using System.Reactive.Linq;
using System.Text;
using AzurePlayground.EventStore.Connection;
using AzurePlayground.Service.Shared;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace AzurePlayground.EventStore.Infrastructure
{
    public static class EventStoreExtensions
    {

        public static IMutable<TKey, TEntity> GetMutator<TKey, TEntity>(this RecordedEvent evt, Type type) where TEntity : IAggregate
        {
            var eventString = Encoding.UTF8.GetString(evt.Data);
            return (IMutable<TKey, TEntity>)JsonConvert.DeserializeObject(eventString, type);
        }

        public static IObservable<IConnected<IEventStoreConnection>> GetEventStoreConnectedStream(this ConnectionStatusMonitor monitor, IEventStoreConnection connection)
        {
            return monitor.ConnectionInfoChanged
                          .Where(con => con.Status == ConnectionStatus.Connected || con.Status == ConnectionStatus.Disconnected)
                          .Select(con => con.Status == ConnectionStatus.Connected ? Connected.Yes(connection) : Connected.No<IEventStoreConnection>());
        }
    }
}