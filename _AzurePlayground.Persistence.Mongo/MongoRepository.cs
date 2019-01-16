using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AzurePlayground.Persistence.Mongo
{

    public class MongoRepository : IMongoRepository
    {
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;

        public MongoRepository(IOptions<MongoConfigurationOptions> options)
        {
            var configuration = options.Value;

            _client = new MongoClient(configuration.MongoConnectionString);
            _database = _client.GetDatabase(MongoUtil.GetDatabaseFromConnectionString(configuration.MongoConnectionString));
        }

        public IMongoDatabase GetDatabase()
        {
            return _database;
        }

        public IQueryable<T> All<T>() where T : class, new()
        {
            return _database.GetCollection<T>(typeof(T).Name).AsQueryable();
        }

        public IQueryable<T> Where<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression) where T : class, new()
        {
            return All<T>().Where(expression);
        }

        public void Delete<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : class, new()
        {
            var result = _database.GetCollection<T>(typeof(T).Name).DeleteMany(predicate);

        }
        public T Single<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression) where T : class, new()
        {
            return All<T>().Where(expression).SingleOrDefault();
        }

        public bool CollectionExists<T>() where T : class, new()
        {
            var collection = _database.GetCollection<T>(typeof(T).Name);
            var totalCount = collection.CountDocuments(new BsonDocument());
            return (totalCount > 0) ? true : false;

        }

        public void Add<T>(T item) where T : class, new()
        {
            _database.GetCollection<T>(typeof(T).Name).InsertOne(item);
        }

        public void Add<T>(IEnumerable<T> items) where T : class, new()
        {
            _database.GetCollection<T>(typeof(T).Name).InsertMany(items);
        }

    }
}
