using System;
using System.Collections.Generic;
using Domainator.Entities;
using Domainator.Infrastructure.Configuration;
using Domainator.Infrastructure.Repositories;
using Domainator.Infrastructure.Repositories.StateManagement.Storage;
using Domainator.Utilities;
using Microsoft.Extensions.DependencyInjection;

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
                services.Add(ServiceDescriptor.Singleton(repositoryType.InterfaceType, repositoryType.ImplementationType));
            }

            return services;
        }
    }
}