using Domainator.Infrastructure.Repositories.StateManagement;
using Domainator.Utilities;

namespace Domainator.Entities
{
    /// <summary>
    /// A base implementation for aggregate roots
    /// </summary>
    public class AbstractAggregateRoot<TEntityId, TAggregateState> : IAggregateRoot
        where TEntityId : class, IEntityIdentity
        where TAggregateState : class, IAggregateState, new()
    {
        protected AbstractAggregateRoot()
        {
            Version = AggregateVersion.Emtpy;
            InternalState = new TAggregateState();
        }

        protected AbstractAggregateRoot(TEntityId id, AggregateVersion version, TAggregateState state)
        {
            Require.NotNull(id, nameof(id));
            Require.NotNull(state, nameof(state));

            InternalId = id;
            InternalState = state;
            Version = version;
        }

        /// <summary>
        /// Gets strongly typed unique identity of the aggregate root entity.
        /// </summary>
        protected TEntityId InternalId { get; set; }

        /// <summary>
        /// Gets strongly typed state of the aggregate.
        /// </summary>
        protected TAggregateState InternalState { get; }

        /// <inheritdoc />
        public IEntityIdentity Id => InternalId;

        /// <inheritdoc />
        public IAggregateState State => InternalState;

        /// <inheritdoc />
        public AggregateVersion Version { get; }
    }
}
