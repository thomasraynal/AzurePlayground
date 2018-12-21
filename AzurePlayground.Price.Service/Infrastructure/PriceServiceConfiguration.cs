using Dasein.Core.Lite;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service.Infrastructure
{
    public class PriceServiceConfiguration : ServiceHubConfigurationBase
    {
        public override string Name { get; set; }
        public override int Version { get; set; }
    }
}
