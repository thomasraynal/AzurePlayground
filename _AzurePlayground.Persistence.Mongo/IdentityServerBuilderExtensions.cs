﻿using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Microsoft.AspNetCore.Identity;

namespace AzurePlayground.Persistence.Mongo
{
    public static class IdentityServerBuilderExtensions
    {

        //public static IIdentityServerBuilder AddMongoRepository(this IIdentityServerBuilder builder)
        //{
        //    builder.Services.AddTransient<IRepository, MongoRepository>();
        //    builder.Services.AddTransient<IMongoRepository, MongoRepository>();
        //    return builder;
        //}



        public static IIdentityServerBuilder AddPersistedGrants(this IIdentityServerBuilder builder)
        {
            builder.Services.AddSingleton<IPersistedGrantStore, CustomPersistedGrantStore>();
            return builder;
        }

        //public static IIdentityServerBuilder AddMongoDbForAspIdentity<TIdentity, TRole>(this IIdentityServerBuilder builder, IConfiguration configuration, ILookupNormalizer normalizer = null) where
        //     TIdentity : MongoUser where TRole : MongoRole
        //{

        //    //refacto
        //    if (null == normalizer) normalizer = new UpperInvariantLookupNormalizer();

        //    var configurationOptions = configuration.Get<MongoConfigurationOptions>();
        //    var client = new MongoClient(configurationOptions.MongoConnectionString);
        //    var database = client.GetDatabase(MongoUtil.GetDatabaseFromConnectionString(configurationOptions.MongoConnectionString));

        //    var rolesCollection = new IdentityRoleCollection<TRole>(configurationOptions.MongoConnectionString, configurationOptions.RolesCollection);
        //    var usersCollection = new IdentityUserCollection<TIdentity>(configurationOptions.MongoConnectionString, configurationOptions.UsersCollection);

        //    builder.Services.AddSingleton<IUserStore<TIdentity>>(x =>
        //    {
        //        IndexChecks.EnsureUniqueTextIndex(usersCollection.MongoCollection, (user)=> user.Email).Wait();
        //        IndexChecks.EnsureUniqueTextIndex(usersCollection.MongoCollection, (user) => user.NormalizedUserName).Wait();
        //        return new UserStore<TIdentity,TRole>(usersCollection, rolesCollection, normalizer);
        //    });

        //    builder.Services.AddSingleton<IRoleStore<TRole>>(x =>
        //    {
        //        IndexChecks.EnsureUniqueTextIndex(rolesCollection.MongoCollection, (role) => role.NormalizedName).Wait();
        //        return new RoleStore<TRole>(rolesCollection);
        //    });

        //    builder.Services.AddIdentity<TIdentity, TRole>();

        //    return builder;
        //}

        //public static IIdentityServerBuilder AddClients(this IIdentityServerBuilder builder)
        //{
        //    builder.Services.AddTransient<IClientStore, ClientStore>();
        //    builder.Services.AddTransient<ICorsPolicyService, InMemoryCorsPolicyService>();
        //    return builder;
        //}

        //public static IIdentityServerBuilder AddIdentityApiResources(this IIdentityServerBuilder builder)
        //{
        //    builder.Services.AddTransient<IResourceStore, ResourceStore>();
        //    return builder;
        //}

        //public static IIdentityServerBuilder AddPersistedGrants(this IIdentityServerBuilder builder)
        //{
        //    builder.Services.AddSingleton<IPersistedGrantStore, PersistedGrantStore>();
        //    return builder;
        //}

    }
}
