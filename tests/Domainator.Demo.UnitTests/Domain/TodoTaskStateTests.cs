using Domainator.Demo.Domain.Domain;
using Xunit;

namespace Domainator.Demo.UnitTests.Domain
{
    public class TodoTaskStateTests
    {
        private readonly TodoTask.AggregateState _state;

        public TodoTaskStateTests()
        {
            _state = new TodoTask.AggregateState();
        }

        [Theory]
        [TodoTaskTestsData]
        public void WhenTodoTaskCreated_SetsCorrectProperties(TodoTaskCreated taskCreated)
        {
            // act
            _state.When(taskCreated);

            // assert
            Assert.NotNull(_state.ProjectId);
            Assert.Equal(taskCreated.ProjectId, _state.ProjectId);
            Assert.Equal(taskCreated.TaskId, _state.TaskId);
        }

        [Theory]
        [TodoTaskTestsData]
        public void WhenTaskMoved_UpdatesProjectId(TodoTaskMoved taskMoved)
        {
            // act
            _state.When(taskMoved);

            // assert
            Assert.NotNull(_state.ProjectId);
            Assert.Equal(taskMoved.NewProjectId, _state.ProjectId);
        }

        [Theory]
        [TodoTaskTestsData]
        public void WhenTaskCompleted_SetsCompletedState(TodoTaskCompleted taskCompleted)
        {
            // act
            _state.When(taskCompleted);

            // assert
            Assert.Equal(TaskState.Completed, _state.TaskState);
        }
    }
}
