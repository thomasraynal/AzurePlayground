using Dasein.Core.Lite;
using Dasein.Core.Lite.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AzurePlayground.Service.Shared;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Dasein.Core.Lite.Hosting;
using FluentValidation.AspNetCore;

namespace AzurePlayground.Authentication
{
    //refacto : openid config
    //refacto : db persistance
    //remove AddDeveloperSigningCredential

    public class AuthenticationServiceStartup : ServiceStartupBase<AuthenticationServiceConfiguration>
    {
  
        public AuthenticationServiceStartup(IHostingEnvironment env, IConfiguration configuration) : base(env, configuration)
        {
        }

        protected override void ConfigureServicesInternal(IServiceCollection services)
        {
            services.AddSerilog(Configuration);
            services.AddSingleton<ICacheStrategy<MethodCacheObject>, DefaultCacheStrategy<MethodCacheObject>>();

            services.AddIdentityServer(options =>
            {
                options.Events.RaiseSuccessEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
            })
                .AddInMemoryClients(AuthenticationRessources.GetClients(ServiceConfiguration))
                .AddInMemoryIdentityResources(AuthenticationRessources.GetIdentityResources())
                .AddInMemoryApiResources(AuthenticationRessources.GetApiResources(ServiceConfiguration))
                .AddDeveloperSigningCredential()
                //.AddJwtBearerClientAuthentication()
                //.AddAppAuthRedirectUriValidator()
                .AddTestUsers(AuthenticationRessources.Users);

            services.AddTransient<IProfileService, ProfileService>();

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = ServiceConfiguration.Identity;
                    options.RequireHttpsMetadata = false;
                    options.ApiName = AzurePlaygroundConstants.Api.Name;
                    options.ApiSecret = ServiceConfiguration.Key;
                    options.SaveToken = true;
                });

            var jsonSettings = new ServiceJsonSerializerSettings();

            this.AddSwagger(services);

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ValidateModelStateAttribute));
            })
                .RegisterJsonSettings(jsonSettings)
                .AddFluentValidation(config => config.RegisterValidatorsFromAssemblies((assembly) => assembly.FullName.Contains("AzurePlayground")));

        }

        protected override void ConfigureInternal(IApplicationBuilder app)
        {
            app.UseIdentityServer();
            this.UseSwagger(app);
            app.UseMvcWithDefaultRoute();
        }
    }
}
