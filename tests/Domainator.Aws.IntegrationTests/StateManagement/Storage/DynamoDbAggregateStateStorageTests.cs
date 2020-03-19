using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Domainator.Demo.Domain.Domain;
using Domainator.Entities;
using Domainator.Infrastructure.StateManagement;
using Domainator.Infrastructure.StateManagement.Storage;
using Xunit;

namespace Domainator.Aws.IntegrationTests.StateManagement.Storage
{
    public class DynamoDbAggregateStateStorageTests : IClassFixture<AggregateStoreDynamoDbTableFixture>
    {
        private readonly IAggregateStateStorage<TodoTaskState> _stateStorage;

        public DynamoDbAggregateStateStorageTests(AggregateStoreDynamoDbTableFixture fixture)
        {
            _stateStorage = new DynamoDbAggregateStateStorage<TodoTaskState>(fixture.StoreTable);
        }

        [Theory]
        [AutoData]
        public async Task LoadAsync_WhenTheStateDoesNotExists_ReturnsEmptyVersion(TodoTaskId id)
        {
            // act
            var (version, _) = await _stateStorage.LoadAsync(id, CancellationToken.None);

            // assert
            Assert.True(AggregateVersion.IsEmpty(version));
        }

        [Theory]
        [AutoData]
        public async Task LoadAsync_CanReadPersistedState(TodoTaskId id, AggregateVersion originalVersion, TodoTaskState state)
        {
            // arrange
            await _stateStorage.PersistAsync(id, state, originalVersion, CancellationToken.None);

            // act
            var (restoredVersion, restoredState) = await _stateStorage.LoadAsync(id, CancellationToken.None);

            // assert
            Assert.Equal(state.ProjectId, restoredState.ProjectId);
            Assert.Equal(originalVersion, restoredVersion);
        }

        [Theory]
        [AutoData]
        public async Task LoadAsync_WhenStateHasChanges_IncrementsVersion(
            TodoTaskId id,
            TodoTaskCreated todoTaskCreated,
            AggregateVersion originalVersion,
            TodoTaskState state)
        {
            // arrange
            state.Mutate(todoTaskCreated);

            await _stateStorage.PersistAsync(id, state, originalVersion, CancellationToken.None);

            // act
            var (restoredVersion, restoredState) = await _stateStorage.LoadAsync(id, CancellationToken.None);

            // assert
            Assert.Equal(state.ProjectId, restoredState.ProjectId);
            Assert.Equal(originalVersion.Increment(), restoredVersion);
        }

        [Theory]
        [AutoData]
        public async Task LoadAsync_WhenStageWasUpdatedConcurrently_Throws(
            TodoTaskId id,
            AggregateVersion originalVersion,
            TodoTaskState state)
        {
            // arrange
            await _stateStorage.PersistAsync(id, state, originalVersion.Increment(), CancellationToken.None);

            // act && assert
            await Assert.ThrowsAsync<StateWasConcurrentlyUpdatedException>(
                () => _stateStorage.PersistAsync(id, state, originalVersion, CancellationToken.None));
        }
    }
}
