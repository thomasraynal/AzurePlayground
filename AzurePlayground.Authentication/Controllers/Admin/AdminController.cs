using AzurePlayground.Persistence;
using AzurePlayground.Service.Shared;
using IdentityServer4.Events;
using IdentityServer4.Quickstart.UI;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.Quickstart.UI
{
    [Authorize(Policy = AzurePlaygroundConstants.Auth.AdminRolePolicy)]
    public class AdminController : Controller
    {
        private IClientRetrieverStore _clientStore;
        private IPersistedGrantRetrieverStore _persistedGrantStore;
        private IEventService _events;
        private IResourceStore _resources;

        public AdminController(
            IClientRetrieverStore clientStore,
            IEventService events,
            IResourceStore resources,
            IPersistedGrantRetrieverStore persistedGrantStore)
        {
            _clientStore = clientStore;
            _persistedGrantStore = persistedGrantStore;
            _events = events;
            _resources = resources;
            _persistedGrantStore = persistedGrantStore;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View("Index", await BuildViewModelAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Revoke(string clientId)
        {
            await _persistedGrantStore.RemoveAsync(clientId);
            return RedirectToAction("Index");
        }

        private async Task<ClientsViewModel> BuildViewModelAsync()
        {
            var clients = await _clientStore.GetAllClientsAsync();

            var list = new List<ClientViewModel>();

            foreach (var client in clients)
            {
                var clientVm = new ClientViewModel
                {
                    Client = client
                };

                var grants = await _persistedGrantStore.GetAllByClientIdAsync(client.ClientId);

                foreach (var grant in grants)
                {
                    var item = new ClientGrantViewModel()
                    {
                        Created = grant.CreationTime,
                        Expires = grant.Expiration,
                        SubjectId = grant.SubjectId,
                        Type = grant.Type,
                        Id = grant.Key

                    };

                    clientVm.Grants.Add(item);

                }

                list.Add(clientVm);
            }

            return new ClientsViewModel
            {
                Clients = list.Where(vm=> vm.Grants.Any()).ToList()
            };
        }
    }
}

