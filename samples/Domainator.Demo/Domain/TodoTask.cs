using Domainator.Entities;
using Domainator.Utilities;

namespace Domainator.Demo.Domain.Domain
{
    public sealed class TodoTask : IAggregateRoot<TodoTaskId, TodoTaskState>
    {
        private TodoTaskId _id;
        private readonly TodoTaskState _state;

        public TodoTask(TodoTaskState state)
        {
            Require.NotNull(state, nameof(state));

            _state = state;

            Version = AggregateVersion.Emtpy;
        }

        public void Create(ProjectId projectId, TodoTaskId taskId)
        {
            Require.NotNull(projectId, nameof(projectId));
            Require.NotNull(taskId, nameof(taskId));

            _id = taskId;


            _state.Mutate(new TodoTaskCreated(projectId, taskId));
        }

        public IEntityIdentity Id
        {
            get
            {
                Ensure.True(_id != null, $"Aggregate {nameof(TodoTask)} has not been created.");

                return _id;
            }
        }

        public IAggregateState State
        {
            get
            {
                Ensure.True(_state != null, $"Aggregate {nameof(TodoTask)} has not been initialized properly.");

                return _state;
            }
        }

        public AggregateVersion Version { get; }
    }
}
