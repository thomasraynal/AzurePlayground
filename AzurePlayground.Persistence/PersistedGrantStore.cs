using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Persistence
{
    public class PersistedGrantStore : IPersistedGrantStore
    {
        protected IRepository _dbRepository;

        public PersistedGrantStore(IRepository repository)
        {
            _dbRepository = repository;
        }

        public Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            var result = _dbRepository.Where<PersistedGrant>(i => i.SubjectId == subjectId);
            return Task.FromResult(result.AsEnumerable());
        }

        public Task<PersistedGrant> GetAsync(string key)
        {
            var result = _dbRepository.Single<PersistedGrant>(i => i.Key == key);
            return Task.FromResult(result);
        }

        public Task RemoveAllAsync(string subjectId, string clientId)
        {
            _dbRepository.Delete<PersistedGrant>(i => i.SubjectId == subjectId && i.ClientId == clientId);
            return Task.FromResult(0);
        }

        public Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            _dbRepository.Delete<PersistedGrant>(i => i.SubjectId == subjectId && i.ClientId == clientId && i.Type == type);
            return Task.FromResult(0);
        }

        public Task RemoveAsync(string key)
        {
            _dbRepository.Delete<PersistedGrant>(i => i.Key == key);
            return Task.FromResult(0);
        }

        public Task StoreAsync(PersistedGrant grant)
        {
            _dbRepository.Add<PersistedGrant>(grant);
            return Task.FromResult(0);
        }
    }
}
