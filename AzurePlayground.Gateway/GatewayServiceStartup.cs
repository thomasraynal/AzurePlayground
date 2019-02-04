using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace AzurePlayground.Gateway
{
    public class GatewayServiceStartup
    {
        private readonly IConfiguration _configuration;
        private readonly GatewayServiceConfiguration _serviceConfiguration;
        private readonly string _serviceConfigurationNamespace = "serviceConfiguration";

        public GatewayServiceStartup(IConfiguration configuration)
        {
            _configuration = configuration;
            _serviceConfiguration = _configuration.GetSection(_serviceConfigurationNamespace).Get<GatewayServiceConfiguration>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                     .ReadFrom.Configuration(_configuration)
                     .CreateLogger();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSerilog(dispose: true);
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed((host) => true)
                    .AllowCredentials());
            });


            services.AddOcelot(_configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");

            app.UseOcelot().Wait();

            Log.Logger.Information($"Start service [{_serviceConfiguration.Name}]");
        }
    }
}
