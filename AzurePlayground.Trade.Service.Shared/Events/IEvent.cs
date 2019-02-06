using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service.Shared
{
    public interface IEvent
    {
        String Name { get; }
    }

    public interface IMutable<TKey,TEntity>: IEvent where TEntity: IAggregate
    {
        TKey EntityId { get; }
        void Apply(TEntity entity);
    }
}
