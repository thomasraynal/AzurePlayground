using Dasein.Core.Lite;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Authentication
{
    public class AuthenticationServiceConfiguration : ServiceConfigurationBase
    {
        public override string Name { get; set; }
        public override int Version { get; set; }
        public string Identity { get; set; }
        public string MongoConnectionString { get; set; }
        public string MongoDatabase { get; set; }
    }
}
