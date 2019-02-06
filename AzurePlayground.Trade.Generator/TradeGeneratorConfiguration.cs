using AzurePlayground.Service.Shared;
using Dasein.Core.Lite;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Generator
{
    public class TradeGeneratorConfiguration : ServiceHubConfigurationBase
    {
        public string Id { get; set; }
        public override string Name { get; set; }
        public override int Version { get; set; }
        public string Identity { get; set; }
        public string Gateway { get; set; }
        public string EventStore { get; set; }
    }
}
