using Domainator.DomainEvents;
using Domainator.Utilities;

namespace Domainator.Demo.Domain.Domain
{
    public class TodoTaskMoved : IDomainEvent
    {
        public TodoTaskId TaskId { get; }

        public ProjectId OldProjectId { get; }

        public ProjectId NewProjectId { get; }

        public TodoTaskMoved(TodoTaskId taskId, ProjectId oldProjectId, ProjectId newProjectId)
        {
            Require.NotNull(taskId, nameof(taskId));
            Require.NotNull(oldProjectId, nameof(oldProjectId));
            Require.NotNull(newProjectId, nameof(newProjectId));

            TaskId = taskId;
            OldProjectId = oldProjectId;
            NewProjectId = newProjectId;
        }
    }
}
