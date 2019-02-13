using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
using Polly;

namespace AzurePlayground.Service.Shared
{
    public static class ConsulRegistrationConstants
    {
        public const string HealthCheckUrl = "/health";
    }

    public static class ConsulRegistrationExtensions
    {

        public static bool IsSuccessStatusCode(this HttpStatusCode statusCode)
        {
            var asInt = (int)statusCode;
            return asInt >= 200 && asInt <= 299;
        }

        public static IServiceCollection AddConsulRegistration<TConfiguration>(this IServiceCollection services, TConfiguration configuration) where TConfiguration : ICanRegister
        {
            services.AddHealthChecks();

            services.AddSingleton<IHostedService, ConsulRegistrationService<TConfiguration>>();
            
            return services
                .AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
             {
                 consulConfig.Address = new Uri(configuration.Consul);
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
        private readonly double _retryTimeout;

        public const double DefaultRetryTimeoutPolicy = 10000;

        public ConsulRegistrationService(IConsulClient consulClient, TConfiguration serviceConfiguration, IServer server, IApplicationLifetime lifetime)
        {
            _server = server;
            _serviceConfiguration = serviceConfiguration;
            _consulClient = consulClient;
            _lifetime = lifetime;

            _retryTimeout = _serviceConfiguration.RetryTimeout == 0 ? DefaultRetryTimeoutPolicy : _serviceConfiguration.RetryTimeout;

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationTokens = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var address = _server.Features.Get<IServerAddressesFeature>().Addresses.First();
            var uri = new Uri(address);

            Uri hostUri = null;

#if DEBUG
            //if run on dev machine, use provided hosturl
            hostUri = uri;

#elif COMPOSE
              //if run via docker-compose, use the container id, resolved by docker cluster dns
             hostUri = new Uri($"{uri.Scheme}://{_serviceConfiguration.Id}:{uri.Port}");

#else
            //if run on kubernetes, use service name which is resolved by the cluster dns
             hostUri = new Uri($"{uri.Scheme}://{_serviceConfiguration.Name}:{uri.Port}");
#endif



            _registrationID = $"{_serviceConfiguration.Id}-{hostUri.Port}";

            var registration = new AgentServiceRegistration()
            {
                ID = _registrationID,
                Name = _serviceConfiguration.Name,
                Address = hostUri.Host,
                Port = hostUri.Port,
                Checks = new[]{
                    new AgentCheckRegistration()
                        {
                            HTTP = $"{hostUri.Scheme}://{hostUri.Host}:{hostUri.Port}{ConsulRegistrationConstants.HealthCheckUrl}",
                            Notes = $"Checks {ConsulRegistrationConstants.HealthCheckUrl} on host",
                            Timeout = TimeSpan.FromSeconds(3),
                            TLSSkipVerify = true,
                            Interval = TimeSpan.FromSeconds(10)

                        }
                    }
            };
            
            RetryPolicies.WaitAndRetryForever<Exception>(
                attemptDelay: TimeSpan.FromMilliseconds(_retryTimeout),
                onRetry: (ex, timespan) => this.LogError($"Failed to register {_serviceConfiguration.Id} [{hostUri}] in Consul [{_serviceConfiguration.Consul}] - [{ex.Message}]"),
                doTry: () =>
                  {
                      this.LogInformation($"Try registering {_serviceConfiguration.Id} [{hostUri}] in Consul [{_serviceConfiguration.Consul}]");

                      _consulClient.Agent.ServiceDeregister(registration.ID, _cancellationTokens.Token).Wait();
                      _consulClient.Agent.ServiceRegister(registration, _cancellationTokens.Token).Wait();
                  }
                );


            this.LogInformation($"Registered {_serviceConfiguration.Id} [{hostUri}] in Consul [{_serviceConfiguration.Consul}]");

            _lifetime.ApplicationStopping.Register(() =>
            {
                _consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            });


            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokens.Cancel();

            this.LogInformation($"Unregistering {_serviceConfiguration.Id} from Consul [{_serviceConfiguration.Consul}]");

            try
            {
                await _consulClient.Agent.ServiceDeregister(_registrationID, cancellationToken);
                this.LogInformation($"Registering {_serviceConfiguration.Id} in Consul [{_serviceConfiguration.Consul}]");
            }
            catch (Exception ex)
            {
                this.LogError($"Unregistering from Consul [{_serviceConfiguration.Consul}] failed", ex);
            }
        }
    }
}