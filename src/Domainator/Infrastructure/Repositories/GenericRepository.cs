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
        private readonly IAggregateStateStorage _stateStorage;

        public GenericRepository(IAggregateStateStorage stateStorage)
        {
            Require.NotNull(stateStorage, nameof(stateStorage));

            _stateStorage = stateStorage;
        }

        /// <inheritdoc />
        public virtual async Task<TAggregateRoot> FindByIdAsync(TEntityId id, CancellationToken cancellationToken)
        {
            Require.NotNull(id, nameof(id));

            var (version, state) = await _stateStorage.LoadAsync<TAggregateState>(id, cancellationToken);
            if (version == AggregateVersion.Emtpy)
            {
                return null;
            }

            return CreateAggregateInstance(id, version, state);
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
        public async Task<IReadOnlyDictionary<TEntityId, TAggregateRoot>> FindByIdBatchAsync(IReadOnlyList<TEntityId> ids, CancellationToken cancellationToken)
        {
            Require.NotNull(ids, nameof(ids));

            var states = await _stateStorage.LoadBatchAsync<TAggregateState>(ids, cancellationToken);
            var result = new Dictionary<TEntityId, TAggregateRoot>();
            foreach (var entityId in ids)
            {
                if (states.TryGetValue(entityId, out var restoredState))
                {
                    var (version, state) = restoredState;
                    result.Add(entityId, CreateAggregateInstance(entityId, version, state));
                }
            }

            return result;
        }

        /// <inheritdoc />
        public virtual async Task SaveAsync(TAggregateRoot entity, CancellationToken cancellationToken)
        {
            Require.NotNull(entity, nameof(entity));

            if (entity.State.HasChanges())
            {
                await _stateStorage.PersistAsync(
                    id: entity.Id,
                    state: (TAggregateState)entity.State,
                    version: entity.Version,
                    attributes: ExtractCustomAttributes((TAggregateState)entity.State),
                    cancellationToken: cancellationToken);
            }
        }

        protected virtual IReadOnlyDictionary<string, object> ExtractCustomAttributes(TAggregateState state)
        {
            return EmptyDictionary<string, object>.Instance;
        }

        protected async Task<RepositoryQueryResult<TAggregateRoot>> FindByAttributeValueAsync(
            FindByAttributeValueStateQuery query, CancellationToken cancellationToken)
        {
            Require.NotNull(query, nameof(query));

            var queryResult = await _stateStorage.FindByAttributeValueAsync<TAggregateState>(query, cancellationToken);
            var items = new List<TAggregateRoot>(queryResult.States.Count);
            foreach (var restoredState in queryResult.States)
            {
                var id = (TEntityId)Activator.CreateInstance(typeof(TEntityId), restoredState.Key);
                var (version, state) = restoredState.Value;

                items.Add(CreateAggregateInstance(id, version, state));
            }

            return new RepositoryQueryResult<TAggregateRoot>(items, queryResult.PaginationToken);
        }

        private static TAggregateRoot CreateAggregateInstance(TEntityId id, AggregateVersion version, TAggregateState state) =>
            (TAggregateRoot)Activator.CreateInstance(typeof(TAggregateRoot), id, version, state);
    }
}
