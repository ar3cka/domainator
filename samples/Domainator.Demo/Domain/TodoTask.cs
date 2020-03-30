using Domainator.Entities;
using Domainator.Utilities;

namespace Domainator.Demo.Domain.Domain
{
    public sealed partial class TodoTask : AbstractAggregateRoot<TodoTaskId, TodoTask.AggregateState>
    {
        public TodoTask()
        {
        }

        public TodoTask(AggregateState state) : base(state)
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

        public void Complete()
        {
            if (InternalState.TaskState != TaskState.Completed)
            {
                State.Mutate(new TodoTaskCompleted(InternalId));
            }
        }
    }
}
