using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;
using AutoFixture;
using AutoFixture.Xunit2;
using Domainator.Demo.Domain.Domain;
using Domainator.Entities;
using Domainator.Infrastructure.Repositories.StateManagement.Serialization.Json;
using Domainator.Infrastructure.Repositories.StateManagement.Storage;
using FluentAssertions;
using Xunit;

namespace Domainator.Aws.IntegrationTests.Repositories.StateManagement.Storage
{
    public class DynamoDbAggregateStateStorageTests : IClassFixture<AggregateStoreDynamoDbTableFixture>
    {
        private readonly IAggregateStateStorage _stateStorage;
        private readonly Table _storageTable;

        public DynamoDbAggregateStateStorageTests(AggregateStoreDynamoDbTableFixture fixture)
        {
            _stateStorage = new DynamoDbAggregateStateStorage(
                fixture.StoreTable,
                new AggregateStateJsonSerializer());
            _storageTable = fixture.StoreTable;
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
        public async Task LoadAsync_CanReadPersistedState(TodoTaskId id, TodoTask.AggregateState state)
        {
            // arrange
            state.Mutate(new TodoTaskCompleted(id));
            await _stateStorage.PersistAsync(id, state, AggregateVersion.Emtpy, new Dictionary<string, object>(),
                CancellationToken.None);

            // act
            var (restoredVersion, restoredState) =
                await _stateStorage.LoadAsync<TodoTask.AggregateState>(id, CancellationToken.None);

            // assert
            Assert.Equal(state.ProjectId, restoredState.ProjectId);
            Assert.Equal(AggregateVersion.Emtpy.Increment(), restoredVersion);
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

            await _stateStorage.PersistAsync(id, state, AggregateVersion.Emtpy, new Dictionary<string, object>(),
                CancellationToken.None);

            // act
            var (restoredVersion, restoredState) =
                await _stateStorage.LoadAsync<TodoTask.AggregateState>(id, CancellationToken.None);

            // assert
            Assert.Equal(state.ProjectId, restoredState.ProjectId);
            Assert.Equal(AggregateVersion.Emtpy.Increment(), restoredVersion);
        }

        [Theory]
        [DynamoDbAggregateStateStorageTestsData]
        public async Task LoadAsync_WhenStageWasUpdatedConcurrently_Throws(
            TodoTaskId id,
            TodoTask.AggregateState state)
        {
            // arrange
            await _stateStorage.PersistAsync(id, state, AggregateVersion.Emtpy, new Dictionary<string, object>(),
                CancellationToken.None);

            // act && assert
            await Assert.ThrowsAsync<StateWasConcurrentlyUpdatedException>(
                () => _stateStorage.PersistAsync(id, state, AggregateVersion.Emtpy.Increment(),
                    new Dictionary<string, object>(), CancellationToken.None));
        }

        [Theory]
        [DynamoDbAggregateStateStorageTestsData]
        public async Task FindByAttributeValueAsync_ReturnsAllStatesByProjectId(
            [Frozen] ProjectId projectId,
            IFixture fixture,
            TodoTaskId[] ids)
        {
            // arrange
            const string attributeName = "TaskProjectId";
            var states = new Dictionary<TodoTaskId, (AggregateVersion, TodoTask.AggregateState)>();

            foreach (var id in ids)
            {
                var state = fixture.Create<TodoTask.AggregateState>();
                state.Mutate(new TodoTaskCreated(projectId, id));
                var version = AggregateVersion.Emtpy;
                states[id] = (version, state);

                await _stateStorage.PersistAsync(
                    id,
                    state,
                    version,
                    new Dictionary<string, object> {{attributeName, projectId}},
                    CancellationToken.None);
            }

            // act
            var query = new FindByAttributeValueStateQuery(attributeName, projectId, ids.Length);
            var queryResult =
                await _stateStorage.FindByAttributeValueAsync<TodoTask.AggregateState>(query, CancellationToken.None);

            // assert
            foreach (var id in ids)
            {
                var (actualVersion, actualState) = states[id];
                var (restoredVersion, restoredState) = queryResult.States[id];

                Assert.Equal(AggregateVersion.Emtpy.Increment(), restoredVersion);
                Assert.Equal(actualState.ProjectId, restoredState.ProjectId);
                Assert.Equal(actualState.TaskState, restoredState.TaskState);
            }
        }

        [Theory]
        [DynamoDbAggregateStateStorageTestsData]
        public async Task FindByAttributeValueAsync_ReturnsAllStatesInPages(
            [Frozen] ProjectId projectId,
            IFixture fixture,
            TodoTaskId[] ids)
        {
            // arrange
            const string attributeName = "TaskProjectId";
            var states = new Dictionary<TodoTaskId, (AggregateVersion, TodoTask.AggregateState)>();

            foreach (var id in ids)
            {
                var state = fixture.Create<TodoTask.AggregateState>();
                var version = AggregateVersion.Emtpy;
                states[id] = (version, state);

                await _stateStorage.PersistAsync(
                    id,
                    state,
                    version,
                    new Dictionary<string, object> {{attributeName, projectId}},
                    CancellationToken.None);
            }

            // act
            var results =
                new Dictionary<IEntityIdentity, (AggregateVersion, TodoTask.AggregateState)>(EntityIdentity
                    .DefaultComparer);
            FindByAttributeValueStateQueryResult<TodoTask.AggregateState> queryResult = null;
            do
            {
                FindByAttributeValueStateQuery query;
                if (queryResult != null)
                {
                    query = new FindByAttributeValueStateQuery(attributeName, projectId, 2,
                        queryResult.PaginationToken);
                }
                else
                {
                    query = new FindByAttributeValueStateQuery(attributeName, projectId, 2);
                }

                queryResult =
                    await _stateStorage.FindByAttributeValueAsync<TodoTask.AggregateState>(query,
                        CancellationToken.None);
                foreach (var pair in queryResult.States)
                {
                    results[pair.Key] = pair.Value;
                }
            } while (queryResult.PaginationToken != null);

            // assert
            foreach (var id in ids)
            {
                var (actualVersion, actualState) = states[id];
                var (restoredVersion, restoredState) = results[id];

                Assert.Equal(actualVersion, restoredVersion);
                Assert.Equal(actualState.ProjectId, restoredState.ProjectId);
                Assert.Equal(actualState.TaskState, restoredState.TaskState);
            }
        }

        [Theory]
        [DynamoDbAggregateStateStorageTestsData]
        public async Task PersistedAsync_ShouldStoreCreatedAtAndUpdatedAt(TodoTaskId taskId, ProjectId projectId)
        {
            // arrange & act
            TodoTask.AggregateState state = new TodoTask.AggregateState();
            state.Mutate(new TodoTaskCreated(projectId, taskId));
            await _stateStorage.PersistAsync(taskId, state, AggregateVersion.Emtpy, new Dictionary<string, object>(), CancellationToken.None);
            var (createdAt1, updatedAt1) = await GetAuditDatesAsync(taskId);

            var (newVersion, newState) = await _stateStorage.LoadAsync<TodoTask.AggregateState>(taskId, CancellationToken.None);
            newState.Mutate(new TodoTaskCompleted(taskId));
            await _stateStorage.PersistAsync(taskId, newState, newVersion, new Dictionary<string, object>(), CancellationToken.None);
            var ( createdAt2, updatedAt2) = await GetAuditDatesAsync(taskId);

            // assert
            Assert.Equal(createdAt1, createdAt2);
            Assert.True(updatedAt2 > updatedAt1);
            createdAt1.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
            updatedAt2.Should().BeCloseTo(DateTimeOffset.UtcNow, 500);
        }

        private async Task<(DateTimeOffset, DateTimeOffset)> GetAuditDatesAsync(TodoTaskId taskId)
        {
            var getConfig = new GetItemOperationConfig {ConsistentRead = true};
            var primaryKey = taskId.Tag + "_" + taskId.Value;
            const string headSortKeyValue = "HEAD";

            var document = await _storageTable.GetItemAsync(primaryKey, headSortKeyValue, getConfig,
                CancellationToken.None);

            var createdAt1 = DateTimeOffset.FromUnixTimeMilliseconds((long)document["CreatedAt"]);
            var modifiedAt1 = DateTimeOffset.FromUnixTimeMilliseconds((long)document["UpdatedAt"]);

            return (createdAt1, modifiedAt1);
        }
    }
}
