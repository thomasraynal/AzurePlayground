using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service.Shared
{
    public interface ICanRegister
    {
        string Consult { get; set; }
        string Id { get; set; }
        string Name { get; set; }
    }
}
