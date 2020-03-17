using Domainator.Demo.Domain.Domain;
using Domainator.Entities;

namespace Domainator.UnitTests.Entities
{
    public sealed class TestableAbstractAggregateRoot : AbstractAggregateRoot<TodoTaskId, TodoTaskState>
    {
        public TestableAbstractAggregateRoot(TodoTaskId id)
        {
            Id = id;
        }

        public override IEntityIdentity Id { get; }
    }
}
