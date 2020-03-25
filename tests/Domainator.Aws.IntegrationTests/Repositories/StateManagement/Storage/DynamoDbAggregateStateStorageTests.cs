using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domainator.Demo.Domain.Domain;
using Domainator.Entities;
using Domainator.Infrastructure.Repositories.StateManagement.Serialization.Json;
using Domainator.Infrastructure.Repositories.StateManagement.Storage;
using Xunit;

namespace Domainator.Aws.IntegrationTests.Repositories.StateManagement.Storage
{
    public class DynamoDbAggregateStateStorageTests : IClassFixture<AggregateStoreDynamoDbTableFixture>
    {
        private readonly IAggregateStateStorage _stateStorage;

        public DynamoDbAggregateStateStorageTests(AggregateStoreDynamoDbTableFixture fixture)
        {
            _stateStorage = new DynamoDbAggregateStateStorage(
                fixture.StoreTable,
                new AggregateStateJsonSerializer());
        }

        [Theory]
        [DynamoDbAggregateStateStorageTestsData]
        public async Task LoadAsync_WhenTheStateDoesNotExists_ReturnsEmptyVersion(TodoTaskId id)
        {
            // act
            var (version, _) = await _stateStorage.LoadAsync<TodoTask.AggregateState>(id, CancellationToken.None);

            // assert
            Assert.True(AggregateVersion.IsEmpty(version));
        }

        [Theory]
        [DynamoDbAggregateStateStorageTestsData]
        public async Task LoadAsync_CanReadPersistedState(TodoTaskId id, AggregateVersion originalVersion, TodoTask.AggregateState state)
        {
            // arrange
            await _stateStorage.PersistAsync(id, state, originalVersion, new Dictionary<string, object>(), CancellationToken.None);

            // act
            var (restoredVersion, restoredState) = await _stateStorage.LoadAsync<TodoTask.AggregateState>(id, CancellationToken.None);

            // assert
            Assert.Equal(state.ProjectId, restoredState.ProjectId);
            Assert.Equal(originalVersion, restoredVersion);
        }

        [Theory]
        [DynamoDbAggregateStateStorageTestsData]
        public async Task LoadAsync_WhenStateHasChanges_IncrementsVersion(
            TodoTaskId id,
            TodoTaskCreated todoTaskCreated,
            AggregateVersion originalVersion,
            TodoTask.AggregateState state)
        {
            // arrange
            state.Mutate(todoTaskCreated);

            await _stateStorage.PersistAsync(id, state, originalVersion, new Dictionary<string, object>(), CancellationToken.None);

            // act
            var (restoredVersion, restoredState) = await _stateStorage.LoadAsync<TodoTask.AggregateState>(id, CancellationToken.None);

            // assert
            Assert.Equal(state.ProjectId, restoredState.ProjectId);
            Assert.Equal(originalVersion.Increment(), restoredVersion);
        }

        [Theory]
        [DynamoDbAggregateStateStorageTestsData]
        public async Task LoadAsync_WhenStageWasUpdatedConcurrently_Throws(
            TodoTaskId id,
            AggregateVersion originalVersion,
            TodoTask.AggregateState state)
        {
            // arrange
            await _stateStorage.PersistAsync(id, state, originalVersion.Increment(), new Dictionary<string, object>(), CancellationToken.None);

            // act && assert
            await Assert.ThrowsAsync<StateWasConcurrentlyUpdatedException>(
                () => _stateStorage.PersistAsync(id, state, originalVersion, new Dictionary<string, object>(), CancellationToken.None));
        }
    }
}
