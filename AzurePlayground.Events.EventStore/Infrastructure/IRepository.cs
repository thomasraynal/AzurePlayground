using AzurePlayground.Service.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzurePlayground.EventStore.Infrastructure
{
    public interface IEventStoreRepository
    {
        Task<TAggregate> GetById<TAggregate>(object id) where TAggregate : IAggregate, new();
        Task<long> SaveAsync(AggregateBase aggregate, params KeyValuePair<string, string>[] extraHeaders);
    }
}