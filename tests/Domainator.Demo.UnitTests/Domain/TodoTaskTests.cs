using AutoFixture.Xunit2;
using Domainator.Demo.Domain.Domain;
using Xunit;

namespace Domainator.Demo.UnitTests.Domain
{
    public class TodoTaskTests
    {
        private readonly TodoTask.AggregateState _state;
        private readonly TodoTask _task;

        public TodoTaskTests()
        {
            _task = new TodoTask();
            _state = (TodoTask.AggregateState)_task.State;
        }

        [Theory]
        [AutoData]
        public void WhenTodoTaskCreated_ProjectIdPropertyReturnsTheCorrectValue(TodoTaskId taskId, ProjectId projectId)
        {
            // act
            _task.Create(taskId, projectId);

            // assert
            Assert.NotNull(_state.ProjectId);
            Assert.Equal(projectId, _state.ProjectId);

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
