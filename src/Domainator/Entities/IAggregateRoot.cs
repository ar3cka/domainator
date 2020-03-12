namespace Domainator.Entities
{
    /// <summary>
    /// The interface of the aggregate root.
    /// </summary>
    /// <typeparam name="TEntityId">The type of identity of the entity which is the root entity of the aggergate.</typeparam>
    /// <typeparam name="TAggregateState">The type of the state</typeparam>
    public interface IAggregateRoot<TEntityId, TAggregateState>
        where TEntityId : IEntityIdentity
        where TAggregateState : IAggregateState
    {
        /// <summary>
        /// Gets the value of aggregate root identity.
        /// </summary>
        IEntityIdentity Id { get; }

        /// <summary>
        /// Gets the state of the aggregate
        /// </summary>
        IAggregateState State { get; }

        /// <summary>
        /// Gets the version of the aggregate.
        /// </summary>
        AggregateVersion Version { get; }
    }
}
