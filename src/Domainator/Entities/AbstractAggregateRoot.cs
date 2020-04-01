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
        /// <summary>
        /// Convenient constructor for for creating the aggregate instance from the state.
        /// </summary>
        /// <param name="state">The state of the aggregate.</param>
        protected AbstractAggregateRoot(TAggregateState state)
        {
            Require.NotNull(state, nameof(state));

            Version = AggregateVersion.Emtpy;
            InternalState = state;
        }

        /// <summary>
        /// Mandatory constructor. It is during for creating the aggregate instance from restored state.
        /// </summary>
        /// <param name="id">The unique identity of the aggregate.</param>
        /// <param name="version">The version of the aggregate</param>
        /// <param name="state">The state of the aggregate.</param>
        protected AbstractAggregateRoot(TEntityId id, AggregateVersion version, TAggregateState state) : this(state)
        {
            Require.NotNull(id, nameof(id));
            Require.NotNull(state, nameof(state));

            InternalId = id;
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
