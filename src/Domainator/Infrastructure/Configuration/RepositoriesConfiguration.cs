using System;
using Domainator.Entities;
using Domainator.Infrastructure.Repositories;
using Domainator.Utilities;

namespace Domainator.Infrastructure.Configuration
{
    /// <summary>
    /// Implements building methods for repositories configuration
    /// </summary>
    public class RepositoriesConfiguration
    {
        private readonly DomainatorInfrastructureBuilder _builder;
        private readonly Action<RepositoryDescriptor> _registerRepository;

        public RepositoriesConfiguration(DomainatorInfrastructureBuilder builder, Action<RepositoryDescriptor> registerRepository)
        {
            Require.NotNull(builder, nameof(builder));
            Require.NotNull(registerRepository, nameof(registerRepository));

            _builder = builder;
            _registerRepository = registerRepository;
        }

        /// <summary>
        /// Registers <see cref="GenericRepository{TEntityId,TAggregateRoot,TAggregateState}"/> for aggregate root type.
        /// </summary>
        public IDomainatorInfrastructureBuilder ForAggregate<TEntityId, TAggregateRoot, TAggregateState>()
            where TEntityId : class, IEntityIdentity
            where TAggregateRoot : class, IAggregateRoot
            where TAggregateState : class, IAggregateState
        {
            return ForAggregate<TEntityId, TAggregateRoot>(builder =>
            {
                builder.UseRepository<IRepository<TEntityId, TAggregateRoot>, GenericRepository<TEntityId, TAggregateRoot, TAggregateState>>();
            });
        }

        /// <summary>
        /// Registers repository for aggregate root type.
        /// </summary>
        public IDomainatorInfrastructureBuilder ForAggregate<TEntityId, TAggregateRoot>(
            Action<RepositoryDescriptorBuilder<TEntityId, TAggregateRoot>> configure)
            where TEntityId : class, IEntityIdentity
            where TAggregateRoot : class, IAggregateRoot
        {
            Require.NotNull(configure, nameof(configure));

            var repositoryBuilder = new RepositoryDescriptorBuilder<TEntityId, TAggregateRoot>();

            configure(repositoryBuilder);

            _registerRepository(repositoryBuilder.RepositoryDescriptor);

            return _builder;
        }
    }
}
