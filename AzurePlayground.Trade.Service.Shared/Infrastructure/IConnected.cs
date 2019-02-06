using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service.Shared
{
    public interface IConnected<out T>
    {
        T Value { get; }
        bool IsConnected { get; }
    }
}
