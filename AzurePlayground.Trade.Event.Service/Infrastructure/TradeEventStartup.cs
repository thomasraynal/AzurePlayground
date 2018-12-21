using AzurePlayground.Service.Domain;
using AzurePlayground.Service.Infrastructure;
using AzurePlayground.Service.Shared;
using Dasein.Core.Lite;
using Dasein.Core.Lite.Hosting;
using Dasein.Core.Lite.Shared;
using FluentValidation.AspNetCore;
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
            });
        }
    }

    public class TradeEventServiceStartup : ServiceStartupBase<TradeEventServiceConfiguration>
    {
        public TradeEventServiceStartup(IHostingEnvironment env, IConfiguration configuration) : base(env, configuration)
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

        protected override void ConfigureInternal(IApplicationBuilder app)
        {
            app.UseAuthentication();

            app.UseSignalR(routes =>
            {
                routes.MapHub<TradeEventHub>("/hub/trade");
            });

            app.UseResponseCaching();

            app.UseMvc();
        }
    }
}
