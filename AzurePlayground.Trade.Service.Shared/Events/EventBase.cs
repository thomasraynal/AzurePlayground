using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service.Shared
{
    public abstract class EventBase<TKey,TEntity> : IMutable<TKey,TEntity> where TEntity : IAggregate
    {
        public string Name => GetType().FullName;
        public TKey EntityId { get; set; }
        public abstract void Apply(TEntity entity);
    }
}
