using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Persistence.Mongo
{
    public class IdentityRoleCollection<TRole> : IIdentityRoleCollection<TRole> where TRole : MongoRole
    {
        public IMongoCollection<TRole> MongoCollection { get; private set; }

        public IdentityRoleCollection(string connectionString, string collectionName)
        {
            MongoCollection = MongoUtil.FromConnectionString<TRole>(connectionString, collectionName);
        }

        public async Task<TRole> FindByNameAsync(string normalizedName)
        {
            return await MongoCollection.FirstOrDefaultAsync(x => x.NormalizedName == normalizedName);
        }

        public async Task<TRole> FindByIdAsync(string roleId)
        {
            return await MongoCollection.FirstOrDefaultAsync(x => x.Id == roleId);
        }

        public async Task<IEnumerable<TRole>> GetAllAsync()
        {
            return (await MongoCollection.FindAsync(x => true)).ToEnumerable();
        }

        public async Task<TRole> CreateAsync(TRole obj)
        {
            await MongoCollection.InsertOneAsync(obj);
            return obj;
        }

        public Task UpdateAsync(TRole obj) => MongoCollection.ReplaceOneAsync(x => x.Id == obj.Id, obj);

        public Task DeleteAsync(TRole obj) => MongoCollection.DeleteOneAsync(x => x.Id == obj.Id);
    }
}
