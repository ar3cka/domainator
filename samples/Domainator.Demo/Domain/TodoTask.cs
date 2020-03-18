using Domainator.Entities;
using Domainator.Utilities;

namespace Domainator.Demo.Domain.Domain
{
    public sealed class TodoTask : AbstractAggregateRoot<TodoTaskId, TodoTaskState>
    {
        public TodoTask(TodoTaskId id) : base(id)
        {
        }

        public TodoTask(TodoTaskId id, AggregateVersion version, TodoTaskState state) : base(id, version, state)
        {
        }

        public void Create(ProjectId projectId)
        {
            Require.NotNull(projectId, nameof(projectId));

            State.Mutate(new TodoTaskCreated(projectId, Id));
        }
    }
}
