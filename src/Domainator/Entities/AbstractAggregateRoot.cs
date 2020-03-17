using Domainator.Utilities;

namespace Domainator.Entities
{
    /// <summary>
    /// A base implementation for aggregate roots
    /// </summary>
    public abstract class AbstractAggregateRoot<TEntityId, TAggregateState> : IAggregateRoot<TEntityId, TAggregateState>
        where TEntityId : IEntityIdentity
        where TAggregateState : class, IAggregateState, new()
    {
        /// <inheritdoc />
        public abstract IEntityIdentity Id { get; }

        /// <inheritdoc />
        public TAggregateState State { get; private set; } = new TAggregateState();

        /// <inheritdoc />
        public AggregateVersion Version { get; private set; } = AggregateVersion.Emtpy;

        /// <inheritdoc />
        public virtual void RestoreFromState(TAggregateState restoredState, AggregateVersion restoredVersion)
        {
            Require.NotNull(restoredState, nameof(restoredState));
            Require.False(Version > restoredVersion, nameof(restoredVersion), "Version > restoredVersion");

            State = restoredState;
            Version = restoredVersion;
        }
    }
}
