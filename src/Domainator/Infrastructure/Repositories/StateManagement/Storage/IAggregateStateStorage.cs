using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domainator.Entities;

namespace Domainator.Infrastructure.Repositories.StateManagement.Storage
{
    /// <summary>
    /// The interface for external storage for aggregate states
    /// </summary>
    public interface IAggregateStateStorage
    {
        /// <summary>
        /// Loads the state of an aggregate from external storage.
        /// </summary>
        /// <param name="id">The unique identifier of the aggregate root.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="TState">The type of the aggregate state.</typeparam>
        /// <returns>Returns the state and the version of the aggregate. If the state is not found, then the version
        /// is empty <see cref="AggregateVersion.IsEmpty"/></returns>
        Task<(AggregateVersion, TState)> LoadAsync<TState>(IEntityIdentity id, CancellationToken cancellationToken)
            where TState : class, IAggregateState;

        /// <summary>
        /// Loads the states in one batch.
        /// </summary>
        /// <param name="ids">The list of identities to load the states for.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="TState">The type of the aggregate state.</typeparam>
        /// <returns>Returns the map for identities and corresponding states.</returns>
        Task<IReadOnlyDictionary<IEntityIdentity, (AggregateVersion, TState)>> LoadBatchAsync<TState>(
            IReadOnlyCollection<IEntityIdentity> ids, CancellationToken cancellationToken)
            where TState : class, IAggregateState;

        /// <summary>
        /// Finds the states by a custom attribute name and its value.
        /// </summary>
        /// <param name="query">The query object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="TState">The type of the aggregate state.</typeparam>
        Task<FindByAttributeValueStateQueryResult<TState>> FindByAttributeValueAsync<TState>(
            FindByAttributeValueStateQuery query, CancellationToken cancellationToken)
            where TState : class, IAggregateState;

        /// <summary>
        /// Persists the state of an aggregate to external storage. The state version will be incremented on the number of changes in
        /// <see cref="IAggregateState.GetChanges"/>.
        /// </summary>
        /// <param name="id">The unique identifier of the aggregate root.</param>
        /// <param name="state">The state to persist.</param>
        /// <param name="version">The current version of the state.</param>
        /// <param name="attributes">The custom attributes of the state.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="TState">The type of the aggregate state.</typeparam>
        /// <exception cref="StateWasConcurrentlyUpdatedException">The exception is thrown in case the version of
        /// the state does not match with <see cref="version"/>.</exception>
        Task PersistAsync<TState>(
            IEntityIdentity id, TState state, AggregateVersion version, IReadOnlyDictionary<string, object> attributes, CancellationToken cancellationToken)
            where TState : class, IAggregateState;

    }
}
