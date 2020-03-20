using AutoFixture.Xunit2;
using Domainator.Demo.Domain.Domain;
using Xunit;

namespace Domainator.Demo.UnitTests.Domain
{
    public class TodoTaskTests
    {
        private readonly TodoTask _task = new TodoTask();

        [Theory]
        [AutoData]
        public void WhenTodoTaskCreated_ProjectIdPropertyReturnsTheCorrectValue(TodoTaskId taskId, ProjectId projectId)
        {
            // act
            _task.Create(taskId, projectId);

            // assert
            Assert.NotNull(_task.State.ProjectId);
            Assert.Equal(projectId, _task.State.ProjectId);

            Assert.Contains(
                _task.State.GetChanges(),
                domainEvent =>
                    domainEvent is TodoTaskCreated todoTaskCreated &&
                    todoTaskCreated.ProjectId == projectId &&
                    todoTaskCreated.TaskId == _task.Id);
        }

        [Theory]
        [AutoData]
        public void WhenTodoTaskCreated_TaskIdPropertyReturnsTheCorrectValue(
            [Frozen] TodoTaskId taskId, ProjectId projectId)
        {
            // act
            _task.Create(taskId, projectId);

            // assert
            Assert.IsType<TodoTaskId>(_task.Id);
            Assert.Equal(taskId, _task.Id);
        }

        [Theory]
        [AutoData]
        public void WhenTodoTaskCreated_CorrespondingEventIsGenerated(TodoTaskId taskId, ProjectId projectId)
        {
            // act
            _task.Create(taskId, projectId);

            // assert
            Assert.Contains(
                _task.State.GetChanges(),
                domainEvent =>
                    domainEvent is TodoTaskCreated todoTaskCreated &&
                    todoTaskCreated.ProjectId == projectId &&
                    todoTaskCreated.TaskId == _task.Id);
        }
    }
}
