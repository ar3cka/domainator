using Domainator.Entities;
using Domainator.Utilities;

namespace Domainator.Demo.Domain.Domain
{
    public partial class TodoTask
    {
        public TodoTask(TodoTaskId id, AggregateVersion version, AggregateState state) : base(id, version, state)
        {
        }

        public sealed class AggregateState : AbstractAggregateState
        {
            public void When(TodoTaskCreated domainEvent)
            {
                Require.NotNull(domainEvent, nameof(domainEvent));

                TaskId = domainEvent.TaskId;
                ProjectId = domainEvent.ProjectId;
                TaskState = TaskState.Created;
            }

            public void When(TodoTaskMoved domainEvent)
            {
                Require.NotNull(domainEvent, nameof(domainEvent));

                ProjectId = domainEvent.NewProjectId;
            }

            public void When(TodoTaskCompleted domainEvent)
            {
                Require.NotNull(domainEvent, nameof(domainEvent));

                TaskState = TaskState.Completed;
            }

            public TodoTaskId TaskId { get; set; }

            public ProjectId ProjectId { get; set; }

            public TaskState TaskState { get; set; }
        }
    }
}
