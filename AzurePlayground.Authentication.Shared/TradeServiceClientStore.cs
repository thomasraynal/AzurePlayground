using AzurePlayground.Service.Shared;
using Dasein.Core.Lite.Shared;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzurePlayground.Authentication
{
    public class TradeServiceClientStore : IClientStore
    {
        private List<Client> _clients;

        public TradeServiceClientStore(IServiceConfiguration configuration)
        {
            _clients = DbSeed.GetSeedClients(configuration).ToList();
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            return Task.FromResult(_clients.FirstOrDefault(c => c.ClientId == clientId));
        }
    }
}
