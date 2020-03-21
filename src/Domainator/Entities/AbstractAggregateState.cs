using System.Collections.Generic;
using System.Linq;
using Domainator.Utilities;

namespace Domainator.Entities
{
    /// <summary>
    /// Provides a basic implementation of aggregate state.
    /// </summary>
    public class AbstractAggregateState : IAggregateState
    {
        private readonly List<object> _changes = new List<object>();

        protected AbstractAggregateState()
        {
        }

        /// <inheritdoc />
        public virtual void Mutate(object domainEvent)
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
        public IReadOnlyList<object> GetChanges() => _changes;

        /// <inheritdoc />
        public bool IsUpdated => _changes.Any();
    }
}
