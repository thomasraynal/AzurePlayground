using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service.Shared
{
    public interface ICanRegister
    {
        string Consul { get; set; }
        double RetryTimeout { get; set; }
        string Id { get; set; }
        string Name { get; set; }
    }
}
