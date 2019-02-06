using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.EventStore.Infrastructure
{
    public interface IEventStoreConfiguration
    {
        string Host { get; }
        int Port { get; }
    }
}
