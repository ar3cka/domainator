using System;
using System.Threading;
using System.Threading.Tasks;
using Domainator.Entities;
using Domainator.Infrastructure.Repositories.StateManagement.Storage;
using Domainator.Utilities;

namespace Domainator.Infrastructure.Repositories
{
    /// <summary>
    /// Generic implementation of <see cref="IAggregateRootRepository{TEntityId,TAggregateRoot,TAggregateState}"./>
    /// </summary>
    /// <typeparam name="TEntityId">The type of unique identity of the root entity.</typeparam>
    /// <typeparam name="TAggregateRoot">The type of aggregate root entity.</typeparam>
    /// <typeparam name="TAggregateState">The type of the state of of the aggregate.</typeparam>
    public class GenericAggregateRootRepository<TEntityId, TAggregateRoot, TAggregateState> : IAggregateRootRepository<TEntityId, TAggregateRoot>
        where TAggregateRoot: class, IAggregateRoot
        where TAggregateState : IAggregateState
        where TEntityId : class, IEntityIdentity
    {
        private readonly IAggregateStateStorage<TAggregateState> _stateStorage;

        public GenericAggregateRootRepository(IAggregateStateStorage<TAggregateState> stateStorage)
        {
            Require.NotNull(stateStorage, nameof(stateStorage));

            _stateStorage = stateStorage;
        }

        /// <inheritdoc />
        public virtual async Task<TAggregateRoot> FindByIdAsync(TEntityId id, CancellationToken cancellationToken)
        {
            Require.NotNull(id, nameof(id));

            var (version, state) = await _stateStorage.LoadAsync(id, cancellationToken);
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
                await _stateStorage.PersistAsync(entity.Id, (TAggregateState)entity.State, entity.Version, cancellationToken);
            }
        }
    }
}
