using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Persistence
{
    public class ResourceStore : IResourceStore
    {
        protected IRepository _dbRepository;

        public ResourceStore(IRepository repository)
        {
            _dbRepository = repository;
        }

        private IEnumerable<ApiResource> GetAllApiResources()
        {
            return _dbRepository.All<ApiResource>();
        }

        private IEnumerable<IdentityResource> GetAllIdentityResources()
        {
            return _dbRepository.All<IdentityResource>();
        }

        public Task<ApiResource> FindApiResourceAsync(string name)
        {
            return Task.FromResult(_dbRepository.Single<ApiResource>(a => a.Name == name));
        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var list = _dbRepository.Where<ApiResource>(a => a.Scopes.Any(s => scopeNames.Contains(s.Name)));
            return Task.FromResult(list.AsEnumerable());
        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var list = _dbRepository.Where<IdentityResource>(e => scopeNames.Contains(e.Name));
            return Task.FromResult(list.AsEnumerable());
        }

        public Task<Resources> GetAllResourcesAsync()
        {
            var result = new Resources(GetAllIdentityResources(), GetAllApiResources());
            return Task.FromResult(result);
        }
    }
}
