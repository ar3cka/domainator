namespace Domainator.Entities
{
    /// <summary>
    /// The interface of the aggregate root.
    /// </summary>
    public interface IAggregateRoot
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
