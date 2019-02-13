using Dasein.Core.Lite;
using Dasein.Core.Lite.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzurePlayground.Service.Infrastructure
{
    public abstract class ServiceHubGatewayConfiguration : ServiceConfigurationBase, IHubConfiguration
    {
        public HubDescriptor[] Hubs
        {
            get
            {
                if (null == Root) return null;

                var hubs = Root.GetSection($"{ServiceConstants.serviceConfiguration}:hubs").Value;
                return JsonConvert.DeserializeObject<HubDescriptor[]>(hubs);
            }
        }
    }
}
