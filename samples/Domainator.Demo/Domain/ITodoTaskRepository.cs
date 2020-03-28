using System.Threading;
using System.Threading.Tasks;
using Domainator.Entities;
using Domainator.Infrastructure.Repositories;

namespace Domainator.Demo.Domain.Domain
{
    public interface ITodoTaskRepository : IRepository<TodoTaskId, TodoTask>
    {
        Task<RepositoryQueryResult<TodoTask>> FindProjectTasksAsync(ProjectId projectId, string paginationToken, CancellationToken cancellationToken);

        Task<RepositoryQueryResult<TodoTask>> FindProjectTasksAsync(ProjectId projectId, CancellationToken cancellationToken);
    }
}