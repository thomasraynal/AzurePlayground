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
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                    .AddIdentityServerAuthentication(options =>
                    {
                        options.Authority = ServiceConfiguration.Identity;
                        options.ApiName = ServiceConfiguration.Name;
                        options.ApiSecret = ServiceConfiguration.Key;
                        options.EnableCaching = true;
                        options.CacheDuration = TimeSpan.FromMinutes(10);
                        options.Events = new JwtBearerEvents()
                        {
                            OnTokenValidated = tvContext =>
                            {
                                return Task.CompletedTask;
                            }

                        };

                    });

            //  services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)

            //.AddJwtBearer(options =>
            //{
            //    var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ServiceConfiguration.Key));

            //    options.TokenValidationParameters = new TokenValidationParameters()
            //    {
            //        IssuerSigningKey = secret,
            //        ValidIssuer = ServiceConfiguration.Name,
            //        ValidateIssuer = true,
            //        ValidateLifetime = true,
            //        ValidateActor = false,
            //        ValidateAudience = false,
            //    };

            //    options.Events = new JwtBearerEvents()
            //    {
            //        OnAuthenticationFailed = context =>
            //        {
            //            throw new UnauthorizedUserException($"Failed to authenticate user [{context.Exception.Message}]");
            //        }
            //    };
            //})
            //.AddOpenIdConnect("oidc",options =>
            //{
            //    options.Authority = Configuration["serviceConfiguration.identity"];
            //    options.RequireHttpsMetadata = false;
            //    options.ClientId = "AzurePlaygroundUserClient";
            //    options.ClientSecret = Configuration["serviceConfiguration.key"].Sha256();
            //    //options.ResponseType = "code id_token";
            //    //options.Scope.Add("apiApp");
            //    //options.Scope.Add("offline_access");
            //    options.GetClaimsFromUserInfoEndpoint = true;
            //    options.SaveTokens = true;
            //});

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
            this.UseSwagger(app);

            app.UseAuthentication();

            app.UseResponseCaching();

            app.UseMvc();
        }
    }
}
