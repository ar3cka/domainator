using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Domainator.Demo.Domain.Domain;
using Domainator.Demo.Domain.Infrastructure.Repositories;
using Domainator.Entities;
using Domainator.Infrastructure.Repositories.StateManagement.Storage;
using Moq;
using Xunit;

namespace Domainator.UnitTests.Infrastructure.Repositories
{
    public class GenericAggregateRootRepositoryTests
    {
        [Theory]
        [GenericAggregateRootRepositoryTestsData]
        public async Task FindByIdAsync_WhenStateDoesNotExist_ReturnsNull(
            [Frozen] Mock<IAggregateStateStorage> stateStorageMock,
            TodoTaskId todoTaskId,
            TodoTaskRepository repository)
        {
            // arrange
            stateStorageMock
                .Setup(self => self.LoadAsync<TodoTask.AggregateState>(todoTaskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((AggregateVersion.Emtpy, default));

            // act
            var aggregateRoot = await repository.FindByIdAsync(todoTaskId, CancellationToken.None);

            // assert
            Assert.Null(aggregateRoot);
        }

        [Theory]
        [GenericAggregateRootRepositoryTestsData]
        public async Task GetByIdAsync_WhenStateDoesNotExist_ThrowsEntityNotFoundException(
            [Frozen] Mock<IAggregateStateStorage> stateStorageMock,
            TodoTaskId todoTaskId,
            TodoTaskRepository repository)
        {
            // arrange
            stateStorageMock
                .Setup(self => self.LoadAsync<TodoTask.AggregateState>(todoTaskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((AggregateVersion.Emtpy, default));

            // act && assert
            await Assert.ThrowsAsync<EntityNotFoundException>(
                () => repository.GetByIdAsync(todoTaskId, CancellationToken.None));
        }

        [Theory]
        [GenericAggregateRootRepositoryTestsData]
        public async Task GetByIdAsync_WhenStateExists_ReturnsAggregateRootWithRestoredStateAndVersion(
            [Frozen] Mock<IAggregateStateStorage> stateStorageMock,
            AggregateVersion version,
            TodoTask.AggregateState todoTaskState,
            TodoTaskId todoTaskId,
            TodoTaskRepository repository)
        {
            // arrange
            stateStorageMock
                .Setup(self => self.LoadAsync<TodoTask.AggregateState>(todoTaskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((version, todoTaskState));

            // act
            var aggregateRoot = await repository.GetByIdAsync(todoTaskId, CancellationToken.None);

            // assert
            Assert.Same(todoTaskState, aggregateRoot.State);
            Assert.Equal(version, aggregateRoot.Version);
        }

        [Theory]
        [GenericAggregateRootRepositoryTestsData]
        public async Task SaveAsync_WhenTheStateHasChanges_PersistsTheStateAndVersionUsingStorage(
            [Frozen] Mock<IAggregateStateStorage> stateStorageMock,
            ProjectId projectId,
            TodoTask task,
            TodoTaskRepository repository)
        {
            // arrange
            task.MoveToProject(projectId);

            // act
            await repository.SaveAsync(task, CancellationToken.None);

            // assert
            stateStorageMock.Verify(
                self => self.PersistAsync(
                    task.Id,
                    (TodoTask.AggregateState)task.State,
                    task.Version,
                    It.IsAny<IReadOnlyDictionary<string, object>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Theory]
        [GenericAggregateRootRepositoryTestsData]
        public async Task SaveAsync_WhenTheStateHasChanges_SetsCustomAttributes(
            [Frozen] Mock<IAggregateStateStorage> stateStorageMock,
            ProjectId projectId,
            TodoTask task,
            TodoTaskRepository repository)
        {
            // arrange
            task.MoveToProject(projectId);

            // act
            await repository.SaveAsync(task, CancellationToken.None);

            // assert
            var state = (TodoTask.AggregateState)task.State;
            stateStorageMock.Verify(
                self => self.PersistAsync(
                    It.IsAny<TodoTaskId>(),
                    It.IsAny<TodoTask.AggregateState>(),
                    It.IsAny<AggregateVersion>(),
                    It.Is<IReadOnlyDictionary<string, object>>(attributes =>
                        attributes["TodoTaskProject"].Equals(state.ProjectId.Id) &&
                        attributes["TodoTaskState"].Equals((int)state.TaskState)),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Theory]
        [GenericAggregateRootRepositoryTestsData]
        public async Task FindByAttributeValueAsync_ExecutesStateStorageQuery(
            [Frozen] Mock<IAggregateStateStorage> stateStorageMock,
            ProjectId projectId,
            string paginationToken,
            string newPaginationToken,
            TodoTaskId id,
            AggregateVersion version,
            TodoTask.AggregateState state,
            TodoTaskRepository repository)
        {
            // arrange
            var stateQueryResult = new FindByAttributeValueStateQueryResult<TodoTask.AggregateState>(
                new Dictionary<IEntityIdentity, (AggregateVersion, TodoTask.AggregateState)> {
                {
                    id, (version, state)
                }},
                newPaginationToken);

            stateStorageMock
                .Setup(self => self.FindByAttributeValueAsync<TodoTask.AggregateState>(
                    It.Is<FindByAttributeValueStateQuery>(query =>
                        query.AttributeName.Equals("TodoTaskProject") &&
                        query.AttributeValue.Equals(projectId.Id) &&
                        query.Limit == 100 &&
                        query.PaginationToken.Equals(paginationToken)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(stateQueryResult);

            // act
            var results = await repository.FindProjectTasksAsync(projectId, paginationToken, CancellationToken.None);

            // assert
            Assert.Contains(
                results.Items,
                task => task.Version == version && task.Id.Equals(id) && ReferenceEquals(task.State, state));

            Assert.Equal(newPaginationToken, results.PaginationToken);
        }

        [Theory]
        [GenericAggregateRootRepositoryTestsData]
        public async Task SaveAsync_WhenTheStateHasNoChanges_SkipPersisting(
            [Frozen] Mock<IAggregateStateStorage> stateStorageMock,
            TodoTask task,
            TodoTaskRepository repository)
        {
            // act
            await repository.SaveAsync(task, CancellationToken.None);

            // assert
            stateStorageMock.Verify(
                self => self.PersistAsync(
                    task.Id,
                    (TodoTask.AggregateState)task.State,
                    task.Version,
                    It.Is<IReadOnlyDictionary<string, object>>(attributes => attributes.Count == 0),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
