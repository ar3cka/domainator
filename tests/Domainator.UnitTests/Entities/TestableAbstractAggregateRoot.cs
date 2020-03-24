using Domainator.Demo.Domain.Domain;
using Domainator.Entities;

namespace Domainator.UnitTests.Entities
{
    public sealed class TestableAbstractAggregateRoot : AbstractAggregateRoot<TodoTaskId, TodoTask.AggregateState>
    {
        public TestableAbstractAggregateRoot(TodoTaskId id, AggregateVersion version, TodoTask.AggregateState state)
            : base(id, version, state)
        {
        }
    }
}
