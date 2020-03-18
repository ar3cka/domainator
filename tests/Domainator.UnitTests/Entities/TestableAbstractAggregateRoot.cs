using Domainator.Demo.Domain.Domain;
using Domainator.Entities;

namespace Domainator.UnitTests.Entities
{
    public sealed class TestableAbstractAggregateRoot : AbstractAggregateRoot<TodoTaskId, TodoTaskState>
    {
        public TestableAbstractAggregateRoot(TodoTaskId id, AggregateVersion version, TodoTaskState state)
            : base(id, version, state)
        {
        }
    }
}
