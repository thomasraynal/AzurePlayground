using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Persistence.Mongo
{
    public static class MongoIdentityExtensions
    {
        public static IIdentityServerBuilder AddIdentityMongoDbProvider<TUser>(this IIdentityServerBuilder builder) 
            where TUser : MongoUser
        {
            return AddIdentityMongoDbProvider<TUser, MongoRole>(builder, x => { });
        }

        public static IIdentityServerBuilder AddIdentityMongoDbProvider<TUser>(this IIdentityServerBuilder builder, Action<MongoConfigurationOptions> setupDatabaseAction) 
            where TUser : MongoUser
        {
            return AddIdentityMongoDbProvider<TUser, MongoRole>(builder, setupDatabaseAction);
        }

        public static IIdentityServerBuilder AddIdentityMongoDbProvider<TUser, TRole>(this IIdentityServerBuilder builder, Action<MongoConfigurationOptions> setupDatabaseAction)
            where TUser : MongoUser
            where TRole : MongoRole
        {
            return AddIdentityMongoDbProvider<TUser, TRole>(builder, x => { }, setupDatabaseAction);
        }

        public static IIdentityServerBuilder AddIdentityMongoDbProvider<TUser, TRole>(this IIdentityServerBuilder builder, Action<IdentityOptions> setupIdentityAction, Action<MongoConfigurationOptions> setupDatabaseAction) 
            where TUser : MongoUser
            where TRole : MongoRole
        {

          //  builder.Services.AddTransient<ICorsPolicyService, InMemoryCorsPolicyService>();

            builder.Services.AddIdentity<TUser, TRole>(setupIdentityAction ?? (x => { }))
                .AddRoleStore<RoleStore<TRole>>()
                .AddUserStore<UserStore<TUser, TRole>>()
                .AddDefaultTokenProviders();

            var dbOptions = new MongoConfigurationOptions();
            setupDatabaseAction(dbOptions);

            var userCollection = new IdentityUserCollection<TUser>(dbOptions.MongoConnectionString, dbOptions.UsersCollection);
            var roleCollection = new IdentityRoleCollection<TRole>(dbOptions.MongoConnectionString, dbOptions.RolesCollection);

            builder.Services.AddTransient<IIdentityUserCollection<TUser>>(x => userCollection);
            builder.Services.AddTransient<IIdentityRoleCollection<TRole>>(x => roleCollection);

            // Identity Services
            builder.Services.AddTransient<IUserStore<TUser>>(x => new UserStore<TUser, TRole>(userCollection, roleCollection, x.GetService<ILookupNormalizer>()));
            builder.Services.AddTransient<IRoleStore<TRole>>(x => new RoleStore<TRole>(roleCollection));

            builder.Services.AddTransient<IRepository, MongoRepository>();
            builder.Services.AddTransient<IMongoRepository, MongoRepository>();

          

            return builder;
        }
    }
}
