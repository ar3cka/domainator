using Domainator.Utilities;

namespace Domainator.Demo.Domain.Domain
{
    public class TodoTaskCreated
    {
        public ProjectId ProjectId { get; }

        public TodoTaskId TaskId { get; }

        public TodoTaskCreated(ProjectId projectId, TodoTaskId taskId)
        {
            Require.NotNull(projectId, nameof(projectId));
            Require.NotNull(taskId, nameof(taskId));

            ProjectId = projectId;
            TaskId = taskId;
        }
    }
}
