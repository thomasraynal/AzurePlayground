using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AzurePlayground.Persistence.Mongo
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddMongoStores(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IRepository, InternalMongoRepository>();

            builder.AddMongoClientStore();
            builder.AddMongoIdentityApiResourceStore();
            builder.AddMongoPersistedGrantStore();
            return builder;
        }

        public static IIdentityServerBuilder AddMongoClientStore(this IIdentityServerBuilder builder)
        {
            BsonClassMap.RegisterClassMap<Client>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
            });

            builder.Services.AddTransient<IClientStore, ClientStore>();
            builder.Services.AddTransient<ICorsPolicyService, InMemoryCorsPolicyService>();
            return builder;
        }

        public static IIdentityServerBuilder AddMongoIdentityApiResourceStore(this IIdentityServerBuilder builder)
        {
            BsonClassMap.RegisterClassMap<IdentityResource>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
            });

            BsonClassMap.RegisterClassMap<ApiResource>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
            });

            builder.Services.AddTransient<IResourceStore, ResourceStore>();
            return builder;
        }

        public static IIdentityServerBuilder AddMongoPersistedGrantStore(this IIdentityServerBuilder builder)
        {
            BsonClassMap.RegisterClassMap<PersistedGrant>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.SetIgnoreExtraElementsIsInherited(true);
            });

            builder.Services.AddSingleton<IPersistedGrantStore, PersistedGrantStore>();
            return builder;
        }
    }
}
