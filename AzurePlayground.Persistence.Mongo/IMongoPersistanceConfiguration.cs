using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Persistence.Mongo
{
    public interface IMongoPersistanceConfiguration
    {
         string MongoConnection { get; set; }
         string MongoDatabaseName { get; set; }
    }
}
