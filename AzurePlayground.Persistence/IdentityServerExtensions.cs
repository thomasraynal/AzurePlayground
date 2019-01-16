using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AzurePlayground.Persistence
{
    public class PasswordHolder<TUser> where TUser : class, new()
    {
        public TUser User { get; private set; }
        public string Password { get; private set; }

        public PasswordHolder(TUser user, string password)
        {
            User = user;
            Password = password;
        }
    }

    public static class IdentityServerExtensions
    {
        public static void SeedApplicationUser<TUser>(this IApplicationBuilder app, IEnumerable<PasswordHolder<TUser>> holders) where TUser : class, new()
        {
            var repository = app.ApplicationServices.GetService<IRepository>();

            repository.ClearTable<TUser>();

            var service = app.ApplicationServices.GetService<UserManager<TUser>>();

            foreach (var holder in holders)
            {
                service.CreateAsync(holder.User, holder.Password).Wait();
            }
        }
        
        public static void SeedIdentityServer(this IApplicationBuilder app, 
            IEnumerable<Client> clients = null, 
            IEnumerable<ApiResource> apiRessources = null,
            IEnumerable<IdentityResource> identityRessources = null)
        {
            var repository = app.ApplicationServices.GetService<IRepository>();

            if (null != clients)
            {
                repository.ClearTable<Client>();

                repository.Add(clients);
            }

            if (null != apiRessources)
            {
                repository.ClearTable<ApiResource>();

                repository.Add(apiRessources);
            }

            if (null != identityRessources)
            {
                repository.ClearTable<IdentityResource>();

                repository.Add(identityRessources);
            }
        }
    }
}
