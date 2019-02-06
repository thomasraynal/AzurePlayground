using AzurePlayground.EventStore.Connection;
using AzurePlayground.EventStore.Infrastructure;
using AzurePlayground.Service.Shared;
using EventStore.ClientAPI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.EventStore
{
    public static class ServiceBuilderExtensions
    {
        public static IServiceCollection AddEventStoreCache<TKey, TCacheItem, TOutput, TEventStoreCache>(this IServiceCollection services) where TEventStoreCache : class, IEventStoreCache<TKey, TCacheItem, TOutput>
        {
            return services.AddSingleton<IEventStoreCache<TKey, TCacheItem, TOutput>, TEventStoreCache>();
        }

        public static IServiceCollection AddEventStore<TRepository>(this IServiceCollection services, string eventStoreUrl) where TRepository : class, IEventStoreRepository
        {
            var configuration = new EventStoreConfiguration(eventStoreUrl);
            var eventStoreConnection = new ExternalEventStore(EventStoreUri.FromConfig(configuration)).Connection;
            eventStoreConnection.ConnectAsync().Wait();

            var monitor = new ConnectionStatusMonitor(eventStoreConnection);
            var eventStoreStream = monitor.GetEventStoreConnectedStream(eventStoreConnection);

            services.AddSingleton<IEventStoreRepository, TRepository>();
            services.Add(new ServiceDescriptor(typeof(IEventStoreConnection), eventStoreConnection));
            services.Add(new ServiceDescriptor(typeof(IObservable<IConnected<IEventStoreConnection>>), eventStoreStream));

            return services;

        }
    }
}
