using System.Collections.Generic;
using Domainator.DomainEvents;
using Domainator.Entities;
using Domainator.Utilities;

namespace Domainator.Infrastructure.Repositories.StateManagement
{
    /// <summary>
    /// The change set of an <see cref="IAggregateRoot"/> changes
    /// </summary>
    public sealed class ChangeSet
    {
        /// <summary>
        /// Gets the initial version of the aggregate.
        /// </summary>
        public AggregateVersion FromVersion { get; }

        /// <summary>
        /// Gets the final version of the aggregate.
        /// </summary>
        public AggregateVersion ToVersion { get; }

        /// <summary>
        /// Gets the list of changes in the aggregate.
        /// </summary>
        public IReadOnlyList<IDomainEvent> Changes { get; }

        public ChangeSet(AggregateVersion fromVersion, IReadOnlyList<IDomainEvent> changes)
        {
            Require.NotNull(changes, nameof(changes));
            
            FromVersion = fromVersion;
            Changes = changes;
            ToVersion = fromVersion.Increment(changes.Count);
        }
    }
}