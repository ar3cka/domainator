using System;
using AutoFixture.Xunit2;
using Domainator.Demo.Domain.Domain;
using Xunit;

namespace Domainator.Demo.UnitTests.Domain
{
    public class TodoTaskTests
    {
        private readonly TodoTaskState _state;
        private readonly TodoTask _todoTask;

        public TodoTaskTests()
        {
            _state = new TodoTaskState();
            _todoTask = new TodoTask(_state);
        }

        [Theory]
        [AutoData]
        public void WhenTodoTaskCreated_ProjectIdPropertyReturnsTheCorrectValue(TodoTaskId taskId, ProjectId projectId)
        {
            // act
            _todoTask.Create(projectId, taskId);

            // assert
            Assert.NotNull(_state.ProjectId);
            Assert.Equal(projectId, _state.ProjectId);

            Assert.Contains(
                _todoTask.State.Changes,
                domainEvent =>
                    domainEvent is TodoTaskCreated todoTaskCreated &&
                    todoTaskCreated.ProjectId == projectId &&
                    todoTaskCreated.TaskId == taskId);
        }

        [Theory]
        [AutoData]
        public void WhenTodoTaskCreated_TaskIdPropertyReturnsTheCorrectValue(TodoTaskId taskId, ProjectId projectId)
        {
            // act
            _todoTask.Create(projectId, taskId);

            // assert
            Assert.IsType<TodoTaskId>(_todoTask.Id);
            Assert.Equal(taskId, _todoTask.Id);
        }

        [Fact]
        public void IdProperty_WhenTaskIsNotCreated_ThrowsInvalidOperationException()
        {
            // act && assert
            Assert.Throws<InvalidOperationException>(() => _todoTask.Id);
        }

        [Theory]
        [AutoData]
        public void WhenTodoTaskCreated_CorrespondingEventIsGenerated(TodoTaskId taskId, ProjectId projectId)
        {
            // act
            _todoTask.Create(projectId, taskId);

            // assert
            Assert.Contains(
                _state.Changes,
                domainEvent =>
                    domainEvent is TodoTaskCreated todoTaskCreated &&
                    todoTaskCreated.ProjectId == projectId &&
                    todoTaskCreated.TaskId == taskId);
        }
    }
}
