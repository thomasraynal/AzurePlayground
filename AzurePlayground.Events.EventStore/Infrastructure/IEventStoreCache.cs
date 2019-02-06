using System;

namespace AzurePlayground.EventStore
{
    public interface IEventStoreCache<TKey, TCacheItem, TOutput>
    {
        CacheState<TKey, TCacheItem> CacheState { get; }

        IObservable<TOutput> GetOutputStream();
    }
}