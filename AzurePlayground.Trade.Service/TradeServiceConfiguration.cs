using Dasein.Core.Lite;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service
{
    public class TradeServiceConfiguration : ServiceHubConfigurationBase
    {
        public override string Name { get; set; }
        public override int Version { get; set; }
    }
}
