using Domainator.Utilities;

namespace Domainator.Entities
{
    /// <summary>
    /// A base implementation for aggregate roots
    /// </summary>
    public class AbstractAggregateRoot<TEntityId, TAggregateState> : IAggregateRoot<TEntityId, TAggregateState>
        where TEntityId : class, IEntityIdentity
        where TAggregateState : class, IAggregateState, new()
    {
        protected AbstractAggregateRoot(TEntityId id)
        {
            Require.NotNull(id, nameof(id));

            Id = id;
            Version = AggregateVersion.Emtpy;
            State = new TAggregateState();
        }

        protected AbstractAggregateRoot(TEntityId id, AggregateVersion version, TAggregateState state) : this(id)
        {
            Require.NotNull(state, nameof(state));

            Version = version;
            State = state;
        }

        /// <inheritdoc />
        public TEntityId Id { get; }

        /// <inheritdoc />
        public TAggregateState State { get; }

        /// <inheritdoc />
        public AggregateVersion Version { get; }
    }
}
