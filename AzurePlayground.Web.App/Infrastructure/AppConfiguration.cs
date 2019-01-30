using Dasein.Core.Lite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzurePlayground.Web.App.Infrastructure
{
    public class AppConfiguration : ServiceConfigurationBase
    {
        public override string Name { get; set; }
        public override int Version { get; set; }
        public string Identity { get; set; }
    }
}
