using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Events.EventStore
{
    public class MutatedEntitiesDto<TEntity>
    {
        public static readonly MutatedEntitiesDto<TEntity> Empty = new MutatedEntitiesDto<TEntity>(new TEntity[0], false, false);

        public MutatedEntitiesDto(IList<TEntity> states, bool isState, bool isStale)
        {
            Trades = states;
            IsCacheState = isState;
            IsStale = isStale;
        }

        public IList<TEntity> Trades { get; }
        public bool IsCacheState { get; }
        public bool IsStale { get; }
    }
}
