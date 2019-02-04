using AzurePlayground.Service.Shared;
using Dasein.Core.Lite;
using Dasein.Core.Lite.Shared;
using FluentValidation.AspNetCore;
using GraphQL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Dasein.Core.Lite.Hosting;
using System.IdentityModel.Tokens.Jwt;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using IdentityServer4.AccessTokenValidation;
using Newtonsoft.Json;

namespace AzurePlayground.Service
{
    public class TradeServiceRegistry : Registry
    {
        public TradeServiceRegistry()
        {
            Scan(scanner =>
            {
                scanner.AssembliesAndExecutablesFromApplicationBaseDirectory();
                scanner.WithDefaultConventions();
                scanner.ConnectImplementationsToTypesClosing(typeof(ISignalRService<,>));
                scanner.ConnectImplementationsToTypesClosing(typeof(IServiceProxy<>)).OnAddedPluginTypes(p => p.Singleton());
            });

            this.RegisterService<ITradeService, TradeService>();
            this.RegisterCacheProxy<IPriceService, PriceService>();

        }
    }

    public class TradeServiceStartup : ServiceStartupBase<TradeServiceConfiguration>
    {
        public TradeServiceStartup(IHostingEnvironment env, IConfiguration configuration) : base(env, configuration)
        {
        }

        protected override void ConfigureServicesInternal(IServiceCollection services)
        {
            services.AddSerilog(Configuration);
            services.AddSingleton<IHubConfiguration>(ServiceConfiguration);
            services.AddSingleton<IHubContextHolder<Price>, HubContextHolder<Price>>();
            services.AddSingleton<IHubContextHolder<TradeEvent>, HubContextHolder<TradeEvent>>();
            services.AddSingleton<ICacheStrategy<MethodCacheObject>, DefaultCacheStrategy<MethodCacheObject>>();
            services.AddSingleton<ICacheStrategy<ResponseCacheEntry>, DefaultCacheStrategy<ResponseCacheEntry>>();
            services.AddTransient<IAuthorizationHandler, ClaimRequirementHandler>();
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<IMemoryCache, ResponseMemoryCache>();

            this.AddSwagger(services);

            services.AddAuthorization(options =>
            {
                options.AddPolicy(TradeServiceReferential.TraderUserPolicy, policy => policy.Requirements.Add(new ClaimRequirement(ClaimTypes.Role, TradeServiceReferential.TraderClaimValue)));
            });

            var jsonSettings = new ServiceJsonSerializerSettings();

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

            services.AddResponseCaching();

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
                         options.ApiName = AzurePlaygroundConstants.Api.Name;
                         options.ApiSecret = ServiceConfiguration.Key;
                     });
        }

    protected override void OnApplicationStart()
        {
            
            Task.Delay(500).Wait();

            var publishers = AppCore.Instance.GetAll<IPublisher>();

            foreach (var publisher in publishers)
            {
                 publisher.Start().Wait();
            }
        }

        protected override void ConfigureInternal(IApplicationBuilder app)
        {
            this.UseSwagger(app);

            app.UseAuthentication();

            app.UseResponseCaching();

            app.UseMvc();
        }
    }
}
