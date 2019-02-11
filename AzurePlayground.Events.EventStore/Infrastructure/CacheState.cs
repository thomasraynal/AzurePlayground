using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.EventStore
{
    public class CacheState<TKey, TCacheItem>
    {
        public CacheState()
        {
            State = new ConcurrentDictionary<TKey, TCacheItem>();
            IsStale = true;
        }

        public bool IsStale { get; set; }

        public ConcurrentDictionary<TKey, TCacheItem> State { get; }
    }
}
