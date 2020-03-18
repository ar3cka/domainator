using Domainator.Entities;
using Domainator.Utilities;

namespace Domainator.Demo.Domain.Domain
{
    public sealed class TodoTaskState : AbstractAggregateState
    {
        public void When(TodoTaskCreated domainEvent)
        {
            Require.NotNull(domainEvent, nameof(domainEvent));

            ProjectId = domainEvent.ProjectId;
        }

        public ProjectId ProjectId { get; set; }
    }
}

