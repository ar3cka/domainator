using Domainator.DomainEvents;
using Domainator.Utilities;

namespace Domainator.Demo.Domain.Domain
{
    public class TodoTaskCompleted : IDomainEvent
    {
        public TodoTaskId TaskId { get; }

        public TodoTaskCompleted(TodoTaskId taskId)
        {
            Require.NotNull(taskId, nameof(taskId));

            TaskId = taskId;
        }
    }
}
