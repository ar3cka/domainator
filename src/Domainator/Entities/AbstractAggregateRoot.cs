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
        protected AbstractAggregateRoot()
        {
            Version = AggregateVersion.Emtpy;
            State = new TAggregateState();
        }

        protected AbstractAggregateRoot(TEntityId id, AggregateVersion version, TAggregateState state)
        {
            Require.NotNull(id, nameof(id));
            Require.NotNull(state, nameof(state));

            Id = id;
            Version = version;
            State = state;
        }

        /// <inheritdoc />
        public TEntityId Id { get; protected set; }

        /// <inheritdoc />
        public TAggregateState State { get; }

        /// <inheritdoc />
        public AggregateVersion Version { get; }
    }
}
