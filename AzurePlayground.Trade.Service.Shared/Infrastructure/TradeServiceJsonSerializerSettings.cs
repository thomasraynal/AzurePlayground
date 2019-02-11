using AzurePlayground.Service.Shared;
using Dasein.Core.Lite.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Service.Shared
{

    public class TradeServiceJsonSerializerSettings : ServiceJsonSerializerSettings
    {
        public TradeServiceJsonSerializerSettings()
        {
            Converters.Add(new AbstractConverter<ITrade,Trade>());
            Converters.Add(new AbstractConverter<IPrice, Price>());
        }
    } 
}
