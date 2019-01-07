using AzurePlayground.Service.Shared;
using Dasein.Core.Lite.Shared;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzurePlayground.Authentication
{
    public class TradeServiceResourceStore : IResourceStore
    {
        private List<ApiResource> _apiRessources;
        private List<IdentityResource> _identityRessources;
        private Resources _ressources;

        public TradeServiceResourceStore(IServiceConfiguration configuration)
        {
            _apiRessources = DbSeed.GetSeedApiResources(configuration).ToList();

            _identityRessources = DbSeed.GetSeedIdentityRessources().ToList();

            _ressources = new Resources(_identityRessources, _apiRessources);
        }

        public Task<ApiResource> FindApiResourceAsync(string name)
        {
            return Task.FromResult(_apiRessources.FirstOrDefault(ressource => ressource.Name == name));
        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            return Task.FromResult(_apiRessources.Where(ressource => ressource.Scopes.Any(scope => scopeNames.Contains(scope.Name))));
        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            return Task.FromResult(_identityRessources.Where(x => scopeNames.Contains(x.Name)));
        }

        public Task<Resources> GetAllResourcesAsync()
        {
            return Task.FromResult(_ressources);
        }
    }
}
