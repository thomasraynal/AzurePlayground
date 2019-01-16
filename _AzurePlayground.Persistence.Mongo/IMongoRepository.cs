using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Persistence.Mongo
{
    public interface IMongoRepository : IRepository
    {
        IMongoDatabase GetDatabase();
    }
}
