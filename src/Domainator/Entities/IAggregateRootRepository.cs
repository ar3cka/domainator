using System.Threading;
using System.Threading.Tasks;

namespace Domainator.Entities
{
    /// <summary>
    /// The interface for the generic aggregate root repository.
    /// </summary>
    /// <typeparam name="TEntityId">The type of unique identity of the root entity.</typeparam>
    /// <typeparam name="TAggregateRoot">The type of aggregate root entity.</typeparam>
    public interface IAggregateRootRepository<in TEntityId, TAggregateRoot>
        where TEntityId : class, IEntityIdentity
        where TAggregateRoot : class, IAggregateRoot
    {
        /// <summary>
        /// Tries to find aggregate root by unique identity
        /// </summary>
        /// <param name="id">The value of unique identity of the entity.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns the aggregate root entity if found, otherwise returns null.</returns>
        Task<TAggregateRoot> FindByIdAsync(TEntityId id, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the aggregate root by its unique identity
        /// </summary>
        /// <param name="id">The value of unique identity of the entity.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns the aggregate root entity.</returns>
        /// <exception cref="EntityNotFoundException">Aggregate root entity with <paramref name="id"/> not found.</exception>
        Task<TAggregateRoot> GetByIdAsync(TEntityId id, CancellationToken cancellationToken);

        /// <summary>
        /// Saves the entity. If entity does not contain changes <see cref="IAggregateState.IsUpdated"/> the entity state
        /// is not persisted.
        /// </summary>
        /// <param name="entity">The entity to save.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task SaveAsync(TAggregateRoot entity, CancellationToken cancellationToken);
    }
}
