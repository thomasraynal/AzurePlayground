using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Persistence.Mongo
{
    public static class MongoIdentityExtensions
    {
        public static IServiceCollection AddIdentityMongoDbProvider<TUser>(this IServiceCollection services) where TUser : MongoUser
        {
            return AddIdentityMongoDbProvider<TUser, MongoRole>(services, x => { });
        }

        public static IServiceCollection AddIdentityMongoDbProvider<TUser>(this IServiceCollection services,
            Action<MongoConfigurationOptions> setupDatabaseAction) where TUser : MongoUser
        {
            return AddIdentityMongoDbProvider<TUser, MongoRole>(services, setupDatabaseAction);
        }

        public static IServiceCollection AddIdentityMongoDbProvider<TUser, TRole>(this IServiceCollection services,
            Action<MongoConfigurationOptions> setupDatabaseAction) where TUser : MongoUser
            where TRole : MongoRole
        {
            return AddIdentityMongoDbProvider<TUser, TRole>(services, x => { }, setupDatabaseAction);
        }

        public static IServiceCollection AddIdentityMongoDbProvider<TUser>(this IServiceCollection services,
            Action<IdentityOptions> setupIdentityAction, Action<MongoConfigurationOptions> setupDatabaseAction) where TUser : MongoUser
        {
            return AddIdentityMongoDbProvider<TUser, MongoRole>(services, setupIdentityAction, setupDatabaseAction);
        }

        public static IServiceCollection AddIdentityMongoDbProvider<TUser, TRole>(this IServiceCollection services,
            Action<IdentityOptions> setupIdentityAction, Action<MongoConfigurationOptions> setupDatabaseAction) where TUser : MongoUser
            where TRole : MongoRole
        {
            services.AddIdentity<TUser, TRole>(setupIdentityAction ?? (x => { }))
                .AddRoleStore<RoleStore<TRole>>()
                .AddUserStore<UserStore<TUser, TRole>>()
                .AddDefaultTokenProviders();

            var dbOptions = new MongoConfigurationOptions();
            setupDatabaseAction(dbOptions);

            var userCollection = new IdentityUserCollection<TUser>(dbOptions.MongoConnection, dbOptions.UsersCollection);
            var roleCollection = new IdentityRoleCollection<TRole>(dbOptions.MongoConnection, dbOptions.RolesCollection);

            services.AddTransient<IIdentityUserCollection<TUser>>(x => userCollection);
            services.AddTransient<IIdentityRoleCollection<TRole>>(x => roleCollection);

            // Identity Services
            services.AddTransient<IUserStore<TUser>>(x => new UserStore<TUser, TRole>(userCollection, roleCollection, x.GetService<ILookupNormalizer>()));
            services.AddTransient<IRoleStore<TRole>>(x => new RoleStore<TRole>(roleCollection));
            return services;
        }
    }
}
