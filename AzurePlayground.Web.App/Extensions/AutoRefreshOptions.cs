using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzurePlayground.Web.App.Extensions
{
    public class AutoRefreshOptions
    {
        public string Scheme { get; set; }
        public TimeSpan RefreshBeforeExpiration { get; set; } = TimeSpan.FromMinutes(1);
    }
}
