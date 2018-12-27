//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using IdentityServer4.EntityFramework.DbContexts;
//using IdentityServer4.EntityFramework.Mappers;
//using System.Linq;
//using Dasein.Core.Lite.Shared;
//using Microsoft.EntityFrameworkCore;

//namespace AzurePlayground.Authentication.Infrastructure
//{
//    public static class IdentityServerDbInitialization
//    {
//        public static void InitializeDatabase()
//        {
//            Seed();
//        }

//        private static void Seed()
//        {
//            var context = AppCore.Instance.Get<ConfigurationDbContext>();
//            var configuration = AppCore.Instance.Get<IServiceConfiguration>();

//            if (!context.Clients.Any())
//            {
//                foreach (var client in AuthenticationRessources.GetClients())
//                {
//                    context.Clients.Add(client.ToEntity());
//                }
//                context.SaveChanges();
//            }

//            if (!context.ApiResources.Any())
//            {
//                foreach (var resource in AuthenticationRessources.GetApiResources())
//                {
//                    context.ApiResources.Add(resource.ToEntity());
//                }
//                context.SaveChanges();
//            }

//            if (!context.ApiResources.Any())
//            {
//                foreach (var resource in AuthenticationRessources.GetIdentityResources())
//                {
//                    context.IdentityResources.Add(resource.ToEntity());
//                }
//                context.SaveChanges();
//            }
//        }
//    }
//}
