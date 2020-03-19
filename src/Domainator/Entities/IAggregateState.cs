using System.Collections.Generic;

namespace Domainator.Entities
{
    /// <summary>
    /// The interface defines the protocol for working with states for aggregates.
    /// </summary>
    public interface IAggregateState
    {
        /// <summary>
        /// Mutates the current state with a domain event.
        /// </summary>
        /// <param name="domainEvent">A domain event.</param>
        void Mutate(object domainEvent);

        /// <summary>
        /// Gets the list of domain events that have been applied to the current state.
        /// </summary>
        IReadOnlyList<object> GetChanges();
    }
}
