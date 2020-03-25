using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domainator.Entities;
using Domainator.Infrastructure.Repositories.StateManagement.Storage;
using Domainator.Utilities;

namespace Domainator.Infrastructure.Repositories
{
    /// <summary>
    /// Generic implementation of <see cref="IRepository{TEntityId,TAggregateRoot}"/>.
    /// </summary>
    /// <typeparam name="TEntityId">The type of unique identity of the root entity.</typeparam>
    /// <typeparam name="TAggregateRoot">The type of aggregate root entity.</typeparam>
    /// <typeparam name="TAggregateState">The type of the state of of the aggregate.</typeparam>
    public class GenericRepository<TEntityId, TAggregateRoot, TAggregateState> : IRepository<TEntityId, TAggregateRoot>
        where TAggregateRoot: class, IAggregateRoot
        where TAggregateState : class, IAggregateState
        where TEntityId : class, IEntityIdentity
    {
        private static readonly IReadOnlyDictionary<string, object> EmptyAttributes = new Dictionary<string, object>(0);

        protected IAggregateStateStorage StateStorage { get; }

        public GenericRepository(IAggregateStateStorage stateStorage)
        {
            Require.NotNull(stateStorage, nameof(stateStorage));

            StateStorage = stateStorage;
        }

        /// <inheritdoc />
        public virtual async Task<TAggregateRoot> FindByIdAsync(TEntityId id, CancellationToken cancellationToken)
        {
            Require.NotNull(id, nameof(id));

            var (version, state) = await StateStorage.LoadAsync<TAggregateState>(id, cancellationToken);
            if (version == AggregateVersion.Emtpy)
            {
                return null;
            }

            return (TAggregateRoot)Activator.CreateInstance(typeof(TAggregateRoot), id, version, state);
        }

        /// <inheritdoc />
        public async Task<TAggregateRoot> GetByIdAsync(TEntityId id, CancellationToken cancellationToken)
        {
            Require.NotNull(id, nameof(id));

            var aggregateRoot = await FindByIdAsync(id, cancellationToken);
            if (aggregateRoot == null)
            {
                throw new EntityNotFoundException($"Aggregate root {id} not found.");
            }

            return aggregateRoot;
        }

        /// <inheritdoc />
        public virtual async Task SaveAsync(TAggregateRoot entity, CancellationToken cancellationToken)
        {
            Require.NotNull(entity, nameof(entity));

            if (entity.State.IsUpdated)
            {
                await StateStorage.PersistAsync(
                    id: entity.Id,
                    state: (TAggregateState)entity.State,
                    version: entity.Version,
                    attributes: ExtractCustomAttributes((TAggregateState)entity.State),
                    cancellationToken: cancellationToken);
            }
        }

        protected virtual IReadOnlyDictionary<string, object> ExtractCustomAttributes(TAggregateState state)
        {
            return EmptyAttributes;
        }
    }
}
