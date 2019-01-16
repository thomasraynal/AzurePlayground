using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbGenericRepository;
using MongoDbGenericRepository.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AzurePlayground.Persistence.Mongo
{
    public class InternalMongoRepository : IRepository
    {
        protected static IMongoDatabase _database;

        public InternalMongoRepository(IMongoDbContext context)
        {
            _database = context.Database;
        }

        private string Pluralize<TDocument>()
        {
            return (typeof(TDocument).Name.Pluralize()).Camelize();
        }

        private String GetCollectionName<TDocument>()
        {
            return Pluralize<TDocument>();
        }

        private IMongoCollection<TDocument> GetCollection<TDocument>()
        {
            var collectionName = GetCollectionName<TDocument>();
            return _database.GetCollection<TDocument>(collectionName);
        }

        public IQueryable<TDocument> All<TDocument>() where TDocument : class, new()
        {
            return GetCollection<TDocument>().AsQueryable();
        }

        public IQueryable<TDocument> Where<TDocument>(Expression<Func<TDocument, bool>> expression) where TDocument : class, new()
        {
            return All<TDocument>().Where(expression);
        }

        public void Delete<TDocument>(Expression<Func<TDocument, bool>> predicate) where TDocument : class, new()
        {
            var result = GetCollection<TDocument>().DeleteMany(predicate);

        }
        public TDocument Single<TDocument>(Expression<Func<TDocument, bool>> expression) where TDocument : class, new()
        {
            return All<TDocument>().Where(expression).SingleOrDefault();
        }

        public bool CollectionExists<TDocument>() where TDocument : class, new()
        {
            var collection = GetCollection<TDocument>();
            var totalCount = collection.CountDocuments(new BsonDocument());
            return (totalCount > 0) ? true : false;
        }

        public void Add<TDocument>(TDocument item) where TDocument : class, new()
        {
            GetCollection<TDocument>().InsertOne(item);
        }

        public void Add<TDocument>(IEnumerable<TDocument> items) where TDocument : class, new()
        {
            GetCollection<TDocument>().InsertMany(items);
        }

        public void ClearTable<TDocument>()
        {
            var collection = GetCollectionName<TDocument>();
            _database.DropCollection(collection);
        }
    }
}
