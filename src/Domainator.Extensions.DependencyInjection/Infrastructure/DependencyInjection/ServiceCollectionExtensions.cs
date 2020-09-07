using System;
using System.Collections.Generic;
using Domainator.Infrastructure.Configuration;
using Domainator.Infrastructure.Repositories.StateManagement.Serialization;
using Domainator.Infrastructure.Repositories.StateManagement.Serialization.Json;
using Domainator.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Domainator.Infrastructure.DependencyInjection
{
    [CLSCompliant(false)]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainatorInfrastructure(
            this IServiceCollection services, Action<IDomainatorInfrastructureBuilder> configure)
        {
            Require.NotNull(services, nameof(services));
            Require.NotNull(configure, nameof(configure));

            var builder = new DomainatorInfrastructureBuilder();

            configure(builder);

            services.AddSingleton(builder.StateStorageFactory);

            services.AddSingleton(
                provider => provider.GetRequiredService<StateStorageFactory>()(provider.GetService));

            foreach (var repositoryType in builder.RegisteredRepositories)
            {
                services.Add(ServiceDescriptor.Scoped(repositoryType.InterfaceType, repositoryType.ImplementationType));
            }

            services.AddSingleton<IAggregateStateSerializer, AggregateStateJsonSerializer>(provider =>
            {
                var converters = new List<JsonConverter>(builder.CustomJsonConverters.Count);
                foreach (var converterType in builder.CustomJsonConverters)
                {
                   converters.Add((JsonConverter)ActivatorUtilities.GetServiceOrCreateInstance(provider, converterType));
                }

                return new AggregateStateJsonSerializer(converters);
            });

            return services;
        }
    }
}
