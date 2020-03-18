using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Domainator.Demo.Domain.Domain;
using Domainator.Entities;
using Domainator.Infrastructure.StateManagement;
using Domainator.Utilities;
using Xunit;

namespace Domainator.Demo.UnitTests.Domain
{
    public class TodoTaskRepository
    {
        private readonly IAggregateStateStorage<TodoTaskState> _stateStorage;

        public TodoTaskRepository(IAggregateStateStorage<TodoTaskState> stateStorage)
        {
            Require.NotNull(stateStorage, nameof(stateStorage));

            _stateStorage = stateStorage;
        }

        public async Task<TodoTask> FindByIdAsync(TodoTaskId id, CancellationToken cancellationToken)
        {
            Require.NotNull(id, nameof(id));

            var (restoredVersion, restoredState) = await _stateStorage.LoadAsync(id, cancellationToken);
            if (restoredVersion == AggregateVersion.Emtpy)
            {
                return null;
            }

            return (TodoTask)Activator.CreateInstance(typeof(TodoTask), id, restoredVersion, restoredState);
        }

        public async Task SaveAsync(TodoTask entity,  CancellationToken cancellationToken)
        {
            await _stateStorage.PersistAsync(entity.Id, entity.State, entity.Version, cancellationToken);
        }
    }

    public class DynamoDbTests
    {
        [Theory]
        [AutoData]
        public async Task Test(TodoTask task, ProjectId projectId)
        {
            var repository = new TodoTaskRepository(new DynamoDbAggregateStateStorage<TodoTaskState>());
            task.Create(projectId);

            await repository.SaveAsync(task, CancellationToken.None);
            var restoredTask = await repository.FindByIdAsync(task.Id, CancellationToken.None);

            Assert.Equal(task.State.ProjectId, restoredTask.State.ProjectId);
        }
    }
}
