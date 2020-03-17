using Domainator.Entities;
using Domainator.Utilities;

namespace Domainator.Demo.Domain.Domain
{
    public sealed class TodoTask : AbstractAggregateRoot<TodoTaskId, TodoTaskState>
    {
        private TodoTaskId _id;

        public void Create(ProjectId projectId, TodoTaskId taskId)
        {
            Require.NotNull(projectId, nameof(projectId));
            Require.NotNull(taskId, nameof(taskId));

            _id = taskId;

            State.Mutate(new TodoTaskCreated(projectId, taskId));
        }

        public override IEntityIdentity Id
        {
            get
            {
                Ensure.True(_id != null, $"Aggregate {nameof(TodoTask)} has not been created.");

                return _id;
            }
        }
    }
}
