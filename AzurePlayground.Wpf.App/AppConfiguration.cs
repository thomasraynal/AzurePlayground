using Dasein.Core.Lite.Shared;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Wpf.App
{
    public class AppConfiguration : IHubConfiguration
    {
        private StringDictionary _args;
        public HubDescriptor[] Hubs { get; }

        public AppConfiguration()
        {
            _args = new StringDictionary();

            var args = Environment.GetCommandLineArgs();

            for (int index = 1; index < args.Length; index += 2)
            {
                _args.Add(args[index].Replace("--", string.Empty), args[index + 1]);
            }

            Hubs = new HubDescriptor[]
            {
                new HubDescriptor()
                {
                    Endpoints= new string[]{ "http://localhost:5003/hub/price" },
                    Name = "PriceHub"
                },

                new HubDescriptor()
                {
                    Endpoints= new string[]{ "http://localhost:5003/hub/trade" },
                    Name = "TradeEventHub"
                }
            };
        }

        public string Identity
        {
            get
            {
                return _args["identity"];
            }
        }

        public string Gateway
        {
            get
            {
                return _args["gateway"];
            }
        }
    }
}
