using Domainator.Entities;
using Domainator.Utilities;

namespace Domainator.Demo.Domain.Domain
{
    public sealed class TodoTask : AbstractAggregateRoot<TodoTaskId, TodoTaskState>
    {
        public TodoTask()
        {
        }

        public TodoTask(TodoTaskId id, AggregateVersion version, TodoTaskState state) : base(id, version, state)
        {
        }

        public void Create(TodoTaskId taskId, ProjectId projectId)
        {
            Require.NotNull(taskId, nameof(taskId));
            Require.NotNull(projectId, nameof(projectId));

            Id = taskId;

            State.Mutate(new TodoTaskCreated(projectId, taskId));
        }

        public void MoveToProject(ProjectId projectId)
        {
            Require.NotNull(projectId, nameof(projectId));

            State.Mutate(new TodoTaskMoved(Id, State.ProjectId, projectId));
        }
    }
}
