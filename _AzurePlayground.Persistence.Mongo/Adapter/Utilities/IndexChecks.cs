using MongoDB.Driver;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AzurePlayground.Persistence.Mongo
{
    public static class IndexChecks
    {
        public static async Task EnsureUniqueTextIndex<TEntity>(IMongoCollection<TEntity> collection, Expression<Func<TEntity, object>> getField)
        {
            var options = new CreateIndexOptions() { Unique = true };
            var builder = Builders<TEntity>.IndexKeys;
            var indexModel = new CreateIndexModel<TEntity>(builder.Ascending(getField), options);
            await collection.Indexes.CreateOneAsync(indexModel);
        }
    }
}
