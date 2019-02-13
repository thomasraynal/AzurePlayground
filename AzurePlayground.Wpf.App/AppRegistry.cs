using AzurePlayground.Service.Domain;
using AzurePlayground.Service.Shared;
using Dasein.Core.Lite.Shared;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Wpf.App
{
    public class AppRegistry : Registry
    {
        public AppRegistry()
        {

            For<ILogger>().Use<VoidLogger>();
            For<IHubConfiguration>().Use<AppConfiguration>();
            For<ISignalRService<Price, PriceRequest>>().Use<PriceHubClient>();
            For<ISignalRService<Trade, TradeEventRequest>>().Use<TradeEventHubClient>();

            var jsonSettings = new TradeServiceJsonSerializerSettings();
            var jsonSerializer = JsonSerializer.Create(jsonSettings);

            For<JsonSerializerSettings>().Use(jsonSettings);
            For<JsonSerializer>().Use(jsonSerializer);

            Scan(scanner =>
            {
                scanner.AssembliesAndExecutablesFromApplicationBaseDirectory();
                scanner.WithDefaultConventions();
                scanner.ConnectImplementationsToTypesClosing(typeof(ISignalRService<,>));
            });
        }
    }
}
