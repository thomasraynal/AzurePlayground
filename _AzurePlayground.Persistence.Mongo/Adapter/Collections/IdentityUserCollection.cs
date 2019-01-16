using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Persistence.Mongo
{
    public class IdentityUserCollection<TUser> : IIdentityUserCollection<TUser> 
        where TUser : MongoUser
    {
        public IMongoCollection<TUser> MongoCollection { get; private set; }

        public IdentityUserCollection(String connectionString, String collection)
        {
            MongoCollection = MongoUtil.FromConnectionString<TUser>(connectionString, collection);
        }

        public async Task<TUser> FindByEmailAsync(string normalizedEmail)
        {
            return await MongoCollection.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);
        }

        public async Task<TUser> FindByUserNameAsync(string username)
        {
            return await MongoCollection.FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<TUser> FindByNormalizedUserNameAsync(string normalizedUserName)
        {
            return await MongoCollection.FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName);
        }

        public async Task<TUser> FindByLoginAsync(string loginProvider, string providerKey)
        {
            return await MongoCollection.FirstOrDefaultAsync(u =>
                u.Logins.Any(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey));
        }

        public async Task<IEnumerable<TUser>> FindUsersByClaimAsync(string claimType, string claimValue)
        {
            return await MongoCollection.WhereAsync(u => u.Claims.Any(c => c.Type == claimType && c.Value == claimValue));
        }

        public async Task<IEnumerable<TUser>> FindUsersInRoleAsync(string roleName)
        {
            var filter = Builders<TUser>.Filter.AnyEq(x => x.Roles, roleName);
            var res = await MongoCollection.FindAsync(filter);
            return res.ToEnumerable();
        }

        public async Task<IEnumerable<TUser>> GetAllAsync()
        {
            var res = await MongoCollection.FindAsync(x => true);
            return res.ToEnumerable();
        }

        public async Task<TUser> CreateAsync(TUser obj)
        {
            await MongoCollection.InsertOneAsync(obj);
            return obj;
        }

        public Task UpdateAsync(TUser obj) => MongoCollection.ReplaceOneAsync(x => x.Id == obj.Id, obj);

        public Task DeleteAsync(TUser obj) => MongoCollection.DeleteOneAsync(x => x.Id == obj.Id);

        public Task<TUser> FindByIdAsync(string itemId) => MongoCollection.FirstOrDefaultAsync(x => x.Id == itemId);
    }
}
