using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Persistence
{
    public class MongoConfigurationOptions
    {
        public string MongoConnection { get; set; }
        public string MongoDatabaseName { get; set; }
        public string UsersCollection { get; set; } = "users";
        public string RolesCollection { get; set; } = "roles";
        public string TradeCollection { get; set; } = "trades";
        public string PriceCollection { get; set; } = "prices";
    }
}
