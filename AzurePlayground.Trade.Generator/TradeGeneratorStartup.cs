using AzurePlayground.Service.Shared;
using Dasein.Core.Lite;
using Dasein.Core.Lite.Hosting;
using Dasein.Core.Lite.Shared;
using EventStore.Client.Lite;
using FluentValidation.AspNetCore;
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
                scanner.ConnectImplementationsToTypesClosing(typeof(IEvent<>));
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

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ValidateModelStateAttribute));
            })
                    .RegisterJsonSettings(new TradeServiceJsonSerializerSettings())
                    .AddFluentValidation(config => config.RegisterValidatorsFromAssemblies((assembly) => assembly.FullName.Contains("AzurePlayground.Service")));


            services.AddEventStore<Guid, EventStoreRepository<Guid>>(ServiceConfiguration.EventStore)
                    .AddEventStoreCache<Guid, Trade>();


        }

        protected override void ConfigureInternal(IApplicationBuilder app)
        {
            app.UseMvc();
        }
    }
}
