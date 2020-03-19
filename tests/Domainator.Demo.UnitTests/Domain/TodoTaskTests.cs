using AutoFixture.Xunit2;
using Domainator.Demo.Domain.Domain;
using Xunit;

namespace Domainator.Demo.UnitTests.Domain
{
    public class TodoTaskTests
    {
        [Theory]
        [AutoData]
        public void WhenTodoTaskCreated_ProjectIdPropertyReturnsTheCorrectValue(TodoTask task, ProjectId projectId)
        {
            // act
            task.Create(projectId);

            // assert
            Assert.NotNull(task.State.ProjectId);
            Assert.Equal(projectId, task.State.ProjectId);

            Assert.Contains(
                task.State.GetChanges(),
                domainEvent =>
                    domainEvent is TodoTaskCreated todoTaskCreated &&
                    todoTaskCreated.ProjectId == projectId &&
                    todoTaskCreated.TaskId == task.Id);
        }

        [Theory]
        [AutoData]
        public void WhenTodoTaskCreated_TaskIdPropertyReturnsTheCorrectValue(
            [Frozen] TodoTaskId taskId, ProjectId projectId, TodoTask task)
        {
            // act
            task.Create(projectId);

            // assert
            Assert.IsType<TodoTaskId>(task.Id);
            Assert.Equal(taskId, task.Id);
        }

        [Theory]
        [AutoData]
        public void WhenTodoTaskCreated_CorrespondingEventIsGenerated(TodoTask task, ProjectId projectId)
        {
            // act
            task.Create(projectId);

            // assert
            Assert.Contains(
                task.State.GetChanges(),
                domainEvent =>
                    domainEvent is TodoTaskCreated todoTaskCreated &&
                    todoTaskCreated.ProjectId == projectId &&
                    todoTaskCreated.TaskId == task.Id);
        }
    }
}
