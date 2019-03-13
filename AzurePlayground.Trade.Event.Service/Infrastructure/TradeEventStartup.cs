using AzurePlayground.Service.Domain;
using AzurePlayground.Service.Shared;
using Dasein.Core.Lite;
using Dasein.Core.Lite.Hosting;
using Dasein.Core.Lite.Shared;
using EventStore.Client.Lite;
using FluentValidation.AspNetCore;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StructureMap;
using System;
using System.Security.Claims;

namespace AzurePlayground.Service.Infrastructure
{
    public class TradeEventServiceRegistry : Registry
    {
        public TradeEventServiceRegistry()
        {
            Scan(scanner =>
            {
                scanner.AssembliesAndExecutablesFromApplicationBaseDirectory();
                scanner.WithDefaultConventions();
                scanner.ConnectImplementationsToTypesClosing(typeof(ISignalRService<,>));
                scanner.ConnectImplementationsToTypesClosing(typeof(IMutable<,>));
                scanner.ConnectImplementationsToTypesClosing(typeof(IEvent<>));
            });
        }
    }

    public class TradeEventServiceStartup : ServiceStartupBase<TradeEventServiceConfiguration>
    {
        public TradeEventServiceStartup(Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IConfiguration configuration) : base(env, configuration)
        {
        }

        protected override void ConfigureServicesInternal(IServiceCollection services)
        {
            services.AddSerilog(Configuration);

            services.AddSingleton<IHubConfiguration>(ServiceConfiguration);
            services.AddSingleton<IHubContextHolder<Trade>, HubContextHolder<Trade>>();
            services.AddSingleton<ICacheStrategy<MethodCacheObject>, DefaultCacheStrategy<MethodCacheObject>>();
            services.AddSingleton<ICacheStrategy<ResponseCacheEntry>, DefaultCacheStrategy<ResponseCacheEntry>>();
            services.AddTransient<IAuthorizationHandler, ClaimRequirementHandler>();
            services.AddSingleton<IMemoryCache, ResponseMemoryCache>();

            services.AddEventStore<Guid, EventStoreRepository<Guid>>(ServiceConfiguration.EventStore)
                    .AddEventStoreCache<Guid, Trade>();

            services.AddSingleton<IHostedService, TradeEventListener>();

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

            if (!HostingEnvironment.IsDevelopment())
            {
                //app.UseHsts();
                //app.UseHttpsRedirection();
            }

            app.UseSignalR(routes =>
            {
                routes.MapHub<TradeEventHub>("/hub/trade");
            });

            app.UseMvc();
        }
    }
}
