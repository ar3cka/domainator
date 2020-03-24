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

                ProjectId = domainEvent.ProjectId;
                TaskState = TaskState.Created;
            }

            public void When(TodoTaskMoved domainEvent)
            {
                Require.NotNull(domainEvent, nameof(domainEvent));

                ProjectId = domainEvent.NewProjectId;
                TaskState = TaskState.Created;
            }

            public ProjectId ProjectId { get; set; }

            public TaskState TaskState { get; set; }
        }
    }
}
