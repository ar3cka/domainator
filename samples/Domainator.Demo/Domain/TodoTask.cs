using Domainator.Entities;
using Domainator.Utilities;

namespace Domainator.Demo.Domain.Domain
{
    public sealed partial class TodoTask : AbstractAggregateRoot<TodoTaskId, TodoTask.AggregateState>
    {
        public TodoTask()
        {
        }

        public void Create(TodoTaskId taskId, ProjectId projectId)
        {
            Require.NotNull(taskId, nameof(taskId));
            Require.NotNull(projectId, nameof(projectId));

            InternalId = taskId;

            State.Mutate(new TodoTaskCreated(projectId, taskId));
        }

        public void MoveToProject(ProjectId projectId)
        {
            Require.NotNull(projectId, nameof(projectId));

            if (projectId.Equals(InternalState.ProjectId))
            {
                return;
            }

            State.Mutate(new TodoTaskMoved(InternalId, InternalState.ProjectId, projectId));
        }
    }
}
