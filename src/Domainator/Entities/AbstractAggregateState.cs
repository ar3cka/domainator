using System.Collections.Generic;
using System.Linq;
using Domainator.DomainEvents;
using Domainator.Utilities;

namespace Domainator.Entities
{
    /// <summary>
    /// Provides a basic implementation of aggregate state.
    /// </summary>
    public class AbstractAggregateState : IAggregateState
    {
        private readonly List<IDomainEvent> _changes = new List<IDomainEvent>();

        protected AbstractAggregateState()
        {
        }

        /// <inheritdoc />
        public virtual void Mutate(IDomainEvent domainEvent)
        {
            Require.NotNull(domainEvent, nameof(domainEvent));

            ApplyEvent(domainEvent);

            _changes.Add(domainEvent);
        }

        private void ApplyEvent(dynamic e)
        {
            (this as dynamic).When(e);
        }

        /// <inheritdoc />
        public IReadOnlyList<IDomainEvent> GetChanges() => _changes;

        /// <inheritdoc />
        public bool HasChanges() => _changes.Any();
    }
}
