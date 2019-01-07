using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Persistence
{
    public class ClientStore : IClientStore
    {
        protected IRepository _dbRepository;

        public ClientStore(IRepository repository)
        {
            _dbRepository = repository;
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = _dbRepository.Single<Client>(c => c.ClientId == clientId);

            return Task.FromResult(client);
        }
    }
}
