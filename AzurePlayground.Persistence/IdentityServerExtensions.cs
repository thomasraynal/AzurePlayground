using AzurePlayground.Service.Shared;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
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

        private static void SeedInternal<TEntity>(IRepository repository, IEnumerable<TEntity> entities, bool forceClear = true) where TEntity : class, new()
        {
            if (forceClear || null != entities)
            {
                repository.ClearTable<TEntity>();
            }

            if (null != entities)
            {
                repository.Add(entities);
            }
        }

        public static void SeedIdentityServer(this IApplicationBuilder app,
            IEnumerable<Client> clients = null,
            IEnumerable<ApiResource> apiRessources = null,
            IEnumerable<IdentityResource> identityRessources = null,
            IEnumerable<PersistedGrant> persistedGrants = null)
        {
            var logger = app.ApplicationServices.GetService<ILogger>();

            RetryPolicies.WaitAndRetryForever<Exception>(
                attemptDelay: TimeSpan.FromMilliseconds(5000),
                onRetry: (ex, timespan) => logger.LogError($"Failed to reach auth db - [{ex.Message}]"),
                doTry: () =>
                    {
                        var repository = app.ApplicationServices.GetService<IRepository>();

                        SeedInternal(repository, clients);
                        SeedInternal(repository, apiRessources);
                        SeedInternal(repository, identityRessources);
                        SeedInternal(repository, persistedGrants);
                    }
                );
        }
    }
}
