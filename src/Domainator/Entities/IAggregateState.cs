using System.Collections.Generic;
using Domainator.DomainEvents;

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
        void Mutate(IDomainEvent domainEvent);

        /// <summary>
        /// Gets the list of domain events that have been applied to the current state.
        /// </summary>
        /// <remarks><see cref="GetChanges"/> is implemented as a method to avoid deserialization.</remarks>
        IReadOnlyList<IDomainEvent> GetChanges();

        /// <summary>
        /// Indicates that the state contains changes
        /// </summary>
        /// <remarks><see cref="HasChanges"/> is implemented as a method to avoid deserialization.</remarks>
        bool HasChanges();
    }
}
