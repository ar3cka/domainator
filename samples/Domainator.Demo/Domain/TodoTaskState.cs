using Domainator.Entities;
using Domainator.Utilities;

namespace Domainator.Demo.Domain.Domain
{
    public enum TaskState
    {
        Created,
        Completed
    }

    public sealed class TodoTaskState : AbstractAggregateState
    {
        public void When(TodoTaskCreated domainEvent)
        {
            Require.NotNull(domainEvent, nameof(domainEvent));

            ProjectId = domainEvent.ProjectId;
            TaskState = TaskState.Created;
        }

        public ProjectId ProjectId { get; set; }

        public TaskState TaskState { get; set; }
    }
}

