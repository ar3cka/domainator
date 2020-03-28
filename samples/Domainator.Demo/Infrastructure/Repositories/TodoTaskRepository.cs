using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domainator.Demo.Domain.Domain;
using Domainator.Infrastructure.Repositories;
using Domainator.Infrastructure.Repositories.StateManagement.Storage;
using Domainator.Utilities;

namespace Domainator.Demo.Domain.Infrastructure.Repositories
{
    public class TodoTaskRepository : GenericRepository<TodoTaskId, TodoTask, TodoTask.AggregateState>, ITodoTaskRepository
    {
        private const string TodoTaskProjectCustomAttribute = "TodoTaskProject";
        private const string TodoTaskStateCustomAttribute = "TodoTaskState";

        public TodoTaskRepository(IAggregateStateStorage stateStorage) : base(stateStorage)
        {
        }

        public async Task<RepositoryQueryResult<TodoTask>> FindProjectTasksAsync(ProjectId projectId, CancellationToken cancellationToken)
        {
            return await FindProjectTasksAsync(projectId, null, cancellationToken);
        }

        public async Task<RepositoryQueryResult<TodoTask>> FindProjectTasksAsync(ProjectId projectId, string paginationToken, CancellationToken cancellationToken)
        {
            Require.NotNull(projectId, nameof(projectId));

            return await FindByAttributeValueAsync(
                new FindByAttributeValueStateQuery(TodoTaskProjectCustomAttribute, projectId.Id, 100, paginationToken), cancellationToken);
        }

        protected override IReadOnlyDictionary<string, object> ExtractCustomAttributes(TodoTask.AggregateState state)
        {
            return new Dictionary<string, object>
            {
                {TodoTaskProjectCustomAttribute, state.ProjectId.Id},
                {TodoTaskStateCustomAttribute, (int)state.TaskState}
            };
        }
    }
}
