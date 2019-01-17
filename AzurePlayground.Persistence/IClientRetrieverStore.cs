using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Persistence
{
    public interface IClientRetrieverStore : IClientStore
    {
        Task<IEnumerable<Client>> GetAllClientsAsync();
    }
}
