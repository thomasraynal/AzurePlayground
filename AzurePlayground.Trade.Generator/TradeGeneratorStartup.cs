using AzurePlayground.Events.EventStore;
using AzurePlayground.EventStore;
using AzurePlayground.EventStore.Infrastructure;
using AzurePlayground.Service.Shared;
using Dasein.Core.Lite;
using Dasein.Core.Lite.Hosting;
using Dasein.Core.Lite.Shared;
using FluentValidation.AspNetCore;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StructureMap;
using System;

namespace AzurePlayground.Generator
{
    public class TradeGeneratorRegistry : Registry
    {
        public TradeGeneratorRegistry()
        {
            Scan(scanner =>
            {
                scanner.AssembliesAndExecutablesFromApplicationBaseDirectory();
                scanner.WithDefaultConventions();
                scanner.AddAllTypesOf<IEvent>();
                scanner.ConnectImplementationsToTypesClosing(typeof(IMutable<,>));
                scanner.ConnectImplementationsToTypesClosing(typeof(ISignalRService<,>));
            });
        }
    }

    public class TradeGeneratorStartup : ServiceStartupBase<TradeGeneratorConfiguration>
    {
        public TradeGeneratorStartup(Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IConfiguration configuration) : base(env, configuration)
        {
        }

        protected override void ConfigureServicesInternal(IServiceCollection services)
        {
            services.AddSerilog(Configuration);

            services.AddSingleton<ICacheStrategy<MethodCacheObject>, DefaultCacheStrategy<MethodCacheObject>>();
            services.AddSingleton<ICacheStrategy<ResponseCacheEntry>, DefaultCacheStrategy<ResponseCacheEntry>>();
            services.AddTransient<IAuthorizationHandler, ClaimRequirementHandler>();

            services.AddSingleton<IHostedService, TradeGenerator>();
           
            var jsonSettings = new ServiceJsonSerializerSettings();
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ValidateModelStateAttribute));
            })
                    .RegisterJsonSettings(jsonSettings)
                    .AddFluentValidation(config => config.RegisterValidatorsFromAssemblies((assembly) => assembly.FullName.Contains("AzurePlayground.Service")));


            services.AddEventStore<EventStoreRepository>(ServiceConfiguration.EventStore)
                    .AddEventStoreCache<Guid, Trade, MutatedEntitiesDto<Trade>, TradeEventCache>();


        }

        protected override void ConfigureInternal(IApplicationBuilder app)
        {
            app.UseMvc();
        }
    }
}
