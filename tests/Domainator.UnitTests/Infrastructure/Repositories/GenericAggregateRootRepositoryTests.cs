using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Domainator.Demo.Domain.Domain;
using Domainator.Entities;
using Domainator.Infrastructure.Repositories;
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
            [Frozen] Mock<IAggregateStateStorage<TodoTask.AggregateState>> stateStorageMock,
            TodoTaskId todoTaskId,
            GenericRepository<TodoTaskId, TodoTask, TodoTask.AggregateState> repository)
        {
            // arrange
            stateStorageMock
                .Setup(self => self.LoadAsync(todoTaskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((AggregateVersion.Emtpy, default));

            // act
            var aggregateRoot = await repository.FindByIdAsync(todoTaskId, CancellationToken.None);

            // assert
            Assert.Null(aggregateRoot);
        }

        [Theory]
        [GenericAggregateRootRepositoryTestsData]
        public async Task GetByIdAsync_WhenStateDoesNotExist_ThrowsEntityNotFoundException(
            [Frozen] Mock<IAggregateStateStorage<TodoTask.AggregateState>> stateStorageMock,
            TodoTaskId todoTaskId,
            GenericRepository<TodoTaskId, TodoTask, TodoTask.AggregateState> repository)
        {
            // arrange
            stateStorageMock
                .Setup(self => self.LoadAsync(todoTaskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((AggregateVersion.Emtpy, default));

            // act && assert
            await Assert.ThrowsAsync<EntityNotFoundException>(
                () => repository.GetByIdAsync(todoTaskId, CancellationToken.None));
        }

        [Theory]
        [GenericAggregateRootRepositoryTestsData]
        public async Task GetByIdAsync_WhenStateExists_ReturnsAggregateRootWithRestoredStateAndVersion(
            [Frozen] Mock<IAggregateStateStorage<TodoTask.AggregateState>> stateStorageMock,
            AggregateVersion version,
            TodoTask.AggregateState todoTaskState,
            TodoTaskId todoTaskId,
            GenericRepository<TodoTaskId, TodoTask, TodoTask.AggregateState> repository)
        {
            // arrange
            stateStorageMock
                .Setup(self => self.LoadAsync(todoTaskId, It.IsAny<CancellationToken>()))
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
            [Frozen] Mock<IAggregateStateStorage<TodoTask.AggregateState>> stateStorageMock,
            ProjectId projectId,
            TodoTask task,
            GenericRepository<TodoTaskId, TodoTask, TodoTask.AggregateState> repository)
        {
            // arrange
            task.MoveToProject(projectId);

            // act
            await repository.SaveAsync(task, CancellationToken.None);

            // assert
            stateStorageMock.Verify(
                self => self.PersistAsync(task.Id, (TodoTask.AggregateState)task.State, task.Version, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Theory]
        [GenericAggregateRootRepositoryTestsData]
        public async Task SaveAsync_WhenTheStateHasNoChanges_SkipPersisting(
            [Frozen] Mock<IAggregateStateStorage<TodoTask.AggregateState>> stateStorageMock,
            TodoTask task,
            GenericRepository<TodoTaskId, TodoTask, TodoTask.AggregateState> repository)
        {
            // act
            await repository.SaveAsync(task, CancellationToken.None);

            // assert
            stateStorageMock.Verify(
                self => self.PersistAsync(task.Id, (TodoTask.AggregateState)task.State, task.Version, It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
