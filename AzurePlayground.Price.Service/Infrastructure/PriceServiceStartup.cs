using AzurePlayground.Service.Domain;
using AzurePlayground.Service.Shared;
using Dasein.Core.Lite;
using Dasein.Core.Lite.Hosting;
using Dasein.Core.Lite.Shared;
using FluentValidation.AspNetCore;
using GraphQL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Service.Infrastructure
{
    public class PriceServiceRegistry : Registry
    {
        public PriceServiceRegistry()
        {
            Scan(scanner =>
            {
                scanner.AssembliesAndExecutablesFromApplicationBaseDirectory();
                scanner.WithDefaultConventions();
                scanner.ConnectImplementationsToTypesClosing(typeof(ISignalRService<,>));
                scanner.ConnectImplementationsToTypesClosing(typeof(IServiceProxy<>)).OnAddedPluginTypes(p => p.Singleton());
            });

            this.For<IPublisher>().Use<PricePublisher>().Singleton();
            this.RegisterCacheProxy<IPriceService, PriceService>();

        }
    }

    public class PriceServiceStartup : ServiceStartupBase<PriceServiceConfiguration>
    {
        public PriceServiceStartup(IHostingEnvironment env, IConfiguration configuration) : base(env, configuration)
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
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<IMemoryCache, ResponseMemoryCache>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ServiceConfiguration.Key));

                        options.TokenValidationParameters = new TokenValidationParameters()
                        {
                            IssuerSigningKey = secret,
                            ValidIssuer = ServiceConfiguration.Name,
                            ValidateIssuer = true,
                            ValidateLifetime = true,
                            ValidateActor = false,
                            ValidateAudience = false,
                        };

                        options.Events = new JwtBearerEvents()
                        {
                            OnAuthenticationFailed = context =>
                            {
                                throw new UnauthorizedUserException($"Failed to authenticate user [{context.Exception.Message}]");
                            }
                        };
                    });

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
            app.UseAuthentication();

            app.UseSignalR(routes =>
            {
                routes.MapHub<PriceHub>("/hub/price");
            });

            app.UseResponseCaching();

            app.UseMvc();
        }
    }
}
