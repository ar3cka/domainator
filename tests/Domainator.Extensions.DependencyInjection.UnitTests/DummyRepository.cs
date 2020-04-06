using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domainator.Demo.Domain.Domain;
using Domainator.Infrastructure.Repositories.StateManagement.Storage;

namespace Domainator.Extensions.DependencyInjection.UnitTests
{
    public class DummyRepository : IDummyRepository
    {
        public IAggregateStateStorage StateStorage { get; }

        public DummyRepository(IAggregateStateStorage stateStorage)
        {
            StateStorage = stateStorage;
        }

        public Task<TodoTask> FindByIdAsync(TodoTaskId id, CancellationToken cancellationToken)
        {
            return null;
        }

        public Task<TodoTask> GetByIdAsync(TodoTaskId id, CancellationToken cancellationToken)
        {
            return null;
        }

        public Task<IReadOnlyList<TodoTask>> FindByIdBatchAsync(IReadOnlyList<TodoTaskId> ids, CancellationToken cancellationToken)
        {
            return null;
        }

        public Task SaveAsync(TodoTask entity, CancellationToken cancellationToken)
        {
            return null;
        }
    }
}
