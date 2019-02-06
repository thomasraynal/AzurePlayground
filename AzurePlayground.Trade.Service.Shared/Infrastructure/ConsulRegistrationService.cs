using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Dasein.Core.Lite.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AzurePlayground.Service.Shared
{
    public static class ConsulRegistrationConstants
    {
        public const string HealthCheckUrl = "/health";
    }

    public static class ConsulRegistrationExtensions
    {

        public static IServiceCollection AddConsulRegistration<TConfiguration>(this IServiceCollection services, TConfiguration configuration) where TConfiguration : ICanRegister
        {
            services.AddHealthChecks();

            services.AddSingleton<IHostedService, ConsulRegistrationService<TConfiguration>>();
            
            return services
                .AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
             {
                 consulConfig.Address = new Uri(configuration.Consult);
             }));

        }

        public static IApplicationBuilder UseConsulRegistration (this IApplicationBuilder app)
        {
            app.UseHealthChecks(ConsulRegistrationConstants.HealthCheckUrl);
            return app;
        }
    }

    public class ConsulRegistrationService<TConfiguration> : IHostedService, ICanLog where TConfiguration : ICanRegister
    {
        private CancellationTokenSource _cancellationTokens;
        private readonly IConsulClient _consulClient;
        private readonly IApplicationLifetime _lifetime;
        private readonly IServer _server;
        private readonly TConfiguration _serviceConfiguration;
        private string _registrationID;

        public ConsulRegistrationService(IConsulClient consulClient, TConfiguration serviceConfiguration, IServer server, IApplicationLifetime lifetime)
        {
            _server = server;
            _serviceConfiguration = serviceConfiguration;
            _consulClient = consulClient;
            _lifetime = lifetime;

        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationTokens = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var features = _server.Features;
            var addresses = features.Get<IServerAddressesFeature>();
            var address = addresses.Addresses.First();

            var uri = new Uri(address);
            _registrationID = $"{_serviceConfiguration.Id}-{uri.Port}";

            var registration = new AgentServiceRegistration()
            {
                ID = _registrationID,
                Name = _serviceConfiguration.Name,
                Address = uri.Host,
                Port = uri.Port,
                Checks = new[]{
                    new AgentCheckRegistration()
                        {
                            HTTP = $"{uri.Scheme}://{uri.Host}:{uri.Port}{ConsulRegistrationConstants.HealthCheckUrl}",
                            Notes = $"Checks {ConsulRegistrationConstants.HealthCheckUrl} on host",
                            Timeout = TimeSpan.FromSeconds(3),
                            Interval = TimeSpan.FromSeconds(10)

                        }
                    }
            };

            this.LogInformation($"Registering {_serviceConfiguration.Id} in Consul");

            await _consulClient.Agent.ServiceDeregister(registration.ID, _cancellationTokens.Token);
            await _consulClient.Agent.ServiceRegister(registration, _cancellationTokens.Token);

            _lifetime.ApplicationStopping.Register(() => {
                _consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            });
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokens.Cancel();

            this.LogInformation($"Unregistering {_serviceConfiguration.Id} from Consul");

            try
            {
                await _consulClient.Agent.ServiceDeregister(_registrationID, cancellationToken);
            }
            catch (Exception ex)
            {
                this.LogError("Deregisteration failed", ex);
            }
        }
    }
}