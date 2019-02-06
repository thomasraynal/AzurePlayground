using AzurePlayground.Service.Shared;
using Dasein.Core.Lite.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzurePlayground.Web.App
{
    public class TradesViewModel
    {
        public IEnumerable<TradeViewModel> Trades { get; set; }
        public string Json
        {
            get
            {
                return JsonConvert.SerializeObject(Trades, AppCore.Instance.Get<JsonSerializerSettings>()); 
            }
        }
    }
}
