using System.Linq;
using AutoFixture.Xunit2;
using Domainator.Demo.Domain.Domain;
using Xunit;

namespace Domainator.Demo.UnitTests.Domain
{
    public class TodoTaskTests
    {
        private readonly TodoTask _task;

        public TodoTaskTests()
        {
            _task = new TodoTask();
        }

        [Theory]
        [TodoTaskTestsData]
        public void Create_TaskIdPropertyReturnsTheCorrectValue(
            [Frozen] TodoTaskId taskId, ProjectId projectId)
        {
            // act
            _task.Create(taskId, projectId);

            // assert
            Assert.IsType<TodoTaskId>(_task.Id);
            Assert.Equal(taskId, _task.Id);
        }

        [Theory]
        [TodoTaskTestsData]
        public void Create_TriggersTodoTaskCreated(TodoTaskId taskId, ProjectId projectId)
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

        [Theory]
        [TodoTaskTestsData]
        public void MoveProject_WhenTheSameProjectIsPassed_DoesNotChangeProject(TodoTaskId taskId, ProjectId projectId)
        {
            // arrange
            _task.Create(taskId, projectId);

            // act
            _task.MoveToProject(projectId);

            // assert
            Assert.DoesNotContain(
                _task.State.GetChanges(),
                domainEvent => domainEvent is TodoTaskMoved);
        }

        [Theory]
        [TodoTaskTestsData]
        public void MoveProject_WhenDifferentProjectIsPassed_ChangesProject(
            TodoTaskId taskId, ProjectId projectId, ProjectId newProjectId)
        {
            // arrange
            _task.Create(taskId, projectId);

            // act
            _task.MoveToProject(newProjectId);

            // assert
            Assert.Contains(
                _task.State.GetChanges(),
                domainEvent =>
                    domainEvent is TodoTaskMoved todoTaskMoved &&
                    todoTaskMoved.OldProjectId == projectId &&
                    todoTaskMoved.NewProjectId == newProjectId &&
                    todoTaskMoved.TaskId == _task.Id);
        }

        [Theory]
        [TodoTaskTestsData]
        public void Complete_TriggersTaskCompletedEvent(TodoTaskId taskId, ProjectId projectId)
        {
            // arrange
            _task.Create(taskId, projectId);

            // act
            _task.Complete();

            // assert
            Assert.Contains(
                _task.State.GetChanges(),
                domainEvent =>
                    domainEvent is TodoTaskCompleted taskCompleted &&
                    taskCompleted.TaskId == _task.Id);
        }

        [Theory]
        [TodoTaskTestsData]
        public void Complete_ForCompletedTask_DoesNotTriggerTaskCompletedEvent(TodoTaskId taskId, ProjectId projectId)
        {
            // arrange
            _task.Create(taskId, projectId);
            _task.Complete();

            // act
            _task.Complete();

            // assert
            Assert.Equal(1, _task.State.GetChanges().Count(evt => evt is TodoTaskCompleted));
        }
    }
}
