using AzurePlayground.Service.Shared;
using Dasein.Core.Lite;
using Dasein.Core.Lite.Shared;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;
using System;
using System.Security.Claims;
using Dasein.Core.Lite.Hosting;
using Microsoft.Extensions.Hosting;

using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using IdentityServer4.AccessTokenValidation;
using AzurePlayground.EventStore;
using AzurePlayground.Events.EventStore;
using AzurePlayground.EventStore.Infrastructure;

namespace AzurePlayground.Service
{
    public class MarketServiceRegistry : Registry
    {
        public MarketServiceRegistry()
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

    public class MarketServiceStartup : ServiceStartupBase<MarketServiceConfiguration>
    {
        public MarketServiceStartup(IHostingEnvironment env, IConfiguration configuration) : base(env, configuration)
        {
        }

        protected override void ConfigureServicesInternal(IServiceCollection services)
        {
            services.AddSerilog(Configuration);

            services.AddSingleton<IHubConfiguration>(ServiceConfiguration);
            services.AddSingleton<IHubContextHolder<Price>, HubContextHolder<Price>>();
            services.AddSingleton<ICacheStrategy<MethodCacheObject>, DefaultCacheStrategy<MethodCacheObject>>();
            services.AddSingleton<ICacheStrategy<ResponseCacheEntry>, DefaultCacheStrategy<ResponseCacheEntry>>();
            services.AddTransient<IAuthorizationHandler, ClaimRequirementHandler>();

            services.AddEventStore<EventStoreRepository>(ServiceConfiguration.EventStore)
                    .AddEventStoreCache<Guid, Trade, MutatedEntitiesDto<Trade>, TradeEventCache>();

            services.AddSingleton<IHostedService, MarketService>();

            services.AddConsulRegistration(ServiceConfiguration);

            services.AddSwagger(ServiceConfiguration);

            services.AddAuthorization(options =>
            {
                options.AddPolicy(TradeServiceReferential.TraderUserPolicy, policy => policy.Requirements.Add(new ClaimRequirement(ClaimTypes.Role, TradeServiceReferential.TraderClaimValue)));
            });

            var jsonSettings = new TradeServiceJsonSerializerSettings();

            services
                    .AddSignalR(hubOptions =>
                    {
                        hubOptions.EnableDetailedErrors = true;
                        hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(1);
                    })
                    .AddJsonProtocol(options =>
                    {
                        options.PayloadSerializerSettings = jsonSettings;
                    });

            services.AddMvc(options =>
                    {
                        options.Filters.Add(typeof(ValidateModelStateAttribute));
                    })
                    .RegisterJsonSettings(jsonSettings)
                    .AddFluentValidation(config => config.RegisterValidatorsFromAssemblies((assembly) => assembly.FullName.Contains("AzurePlayground.Service")));


            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                    .AddIdentityServerAuthentication(options =>
                     {
                         options.RequireHttpsMetadata = false;
                         options.Authority = ServiceConfiguration.Identity;
                         options.ApiName = AzurePlaygroundConstants.Api.Trade;
                         options.ApiSecret = ServiceConfiguration.Key;
                     });
        }

        protected override void ConfigureInternal(IApplicationBuilder app)
        {
            app.UseSwagger(ServiceConfiguration);

            app.UseAuthentication();

            app.UseConsulRegistration();

            app.UseMvc();
        }
    }
}
