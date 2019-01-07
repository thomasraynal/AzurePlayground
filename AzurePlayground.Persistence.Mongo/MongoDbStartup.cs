using System;
using System.Linq;
using AzurePlayground.Authentication;
using Dasein.Core.Lite.Shared;
using IdentityModel;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace AzurePlayground.Persistence.Mongo
{
    public static class MongoDbStartup
    {
        private static string _newRepositoryMsg = $"Mongo Repository created/populated! Please restart your website, so Mongo driver will be configured  to ignore Extra Elements.";

        public static void AddMongoDbConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoConfigurationOptions>(configuration);
        }

        public static void UseMongoDbForIdentityServer(this IApplicationBuilder app)
        {
            var repository = app.ApplicationServices.GetService<IRepository>();
            var configuration = app.ApplicationServices.GetService<IServiceConfiguration>();

            var userManager = app.ApplicationServices.GetService<UserManager<MongoUser>>();

            ConfigureMongoDriver2IgnoreExtraElements();

            var createdNewRepository = false;

            if (!repository.CollectionExists<Client>())
            {
                foreach (var client in DbSeed.GetSeedClients(configuration))
                {
                    repository.Add(client);
                }
                createdNewRepository = true;
            }

            if (!repository.CollectionExists<IdentityResource>())
            {
                foreach (var res in DbSeed.GetSeedIdentityRessources())
                {
                    repository.Add(res);
                }
                createdNewRepository = true;
            }

            if (!repository.CollectionExists<ApiResource>())
            {
                foreach (var api in DbSeed.GetSeedApiResources(configuration))
                {
                    repository.Add(api);
                }
                createdNewRepository = true;
            }

            if (createdNewRepository == true)
            {
                AddSampleUsersToMongo(userManager);
            }

        }

        private static void ConfigureMongoDriver2IgnoreExtraElements()
        {
            BsonClassMap.RegisterClassMap<Client>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<IdentityResource>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<ApiResource>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<PersistedGrant>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
        }


        private static void AddSampleUsersToMongo(UserManager<MongoUser> userManager)
        {
            var dummyUsers = DbSeed.GetSeedUsers();

            foreach (var usrDummy in dummyUsers)
            {


                var userDummyEmail = usrDummy.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Email);

                if (userDummyEmail == null)
                {
                    throw new Exception("Could not locate user email from  claims!");
                }

                var user = new MongoUser()
                {
                    Claims = usrDummy.Claims.ToList(),

                }

                var result = userManager.CreateAsync(usrDummy, usrDummy.Password);
                if (!result.Result.Succeeded)
                {
                    // If we got an error, Make sure to drop all collections from Mongo before trying again. Otherwise sample users will NOT be populated
                    var errorList = result.Result.Errors.ToArray();
                    throw new Exception($"Error Adding sample users to MongoDB! Make sure to drop all collections from Mongo before trying again!");
                }
            }

            return;
        }
    }
}