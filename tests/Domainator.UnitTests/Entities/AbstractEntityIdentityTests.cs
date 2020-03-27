using Domainator.Demo.Domain.Domain;
using Domainator.Entities;
using Xunit;

namespace Domainator.UnitTests.Entities
{
    public class AbstractEntityIdentityTests
    {
        [Theory]
        [AbstractEntityIdentityTestsData]
        public void Equals_ForTheSameTaskReturnsTrue(TodoTaskId taskId)
        {
            Assert.Equal(taskId, taskId);
            Assert.True(taskId == new TodoTaskId(taskId.Id));
            Assert.False(taskId != new TodoTaskId(taskId.Id));
            Assert.True(taskId.Equals(new TodoTaskId(taskId.Id)));
        }

        [Theory]
        [AbstractEntityIdentityTestsData]
        public void Equals_ForDifferentTasksReturnsFalse(TodoTaskId task1, TodoTaskId task2)
        {
            Assert.NotEqual(task1, task2);
            Assert.False(task1 == task2);
            Assert.True(task1 != task2);
            Assert.False(task1.Equals(task2));
        }

        [Theory]
        [AbstractEntityIdentityTestsData]
        public void Equals_ForDifferentAbstractEntityIdentitySubTypesReturnsFalse(TodoTaskId task, ProjectId project)
        {
            var taskIdentity = task as AbstractEntityIdentity<int>;
            var projectIdentity = project as AbstractEntityIdentity<int>;

            Assert.NotEqual(taskIdentity, projectIdentity);
            Assert.False(taskIdentity == projectIdentity);
            Assert.True(taskIdentity != projectIdentity);
            Assert.False(taskIdentity.Equals(projectIdentity));
        }

        [Theory]
        [AbstractEntityIdentityTestsData]
        public void Equals_ForDifferentEntityIdentityTypesReturnsFalse(TodoTaskId task, ProjectId project)
        {
            var taskIdentity = task as IEntityIdentity;
            var projectIdentity = project as IEntityIdentity;

            Assert.NotEqual(taskIdentity, projectIdentity);
            Assert.False(taskIdentity.Equals(projectIdentity));
        }

        [Theory]
        [AbstractEntityIdentityTestsData]
        public void Equals_ForTheSameInstanceReturnsTrue(TodoTaskId task)
        {
            Assert.Equal(task, task);
            Assert.True(task.Equals(task));
        }

        [Theory]
        [AbstractEntityIdentityTestsData]
        public void Equals_WhenTheValueIsComparedWithNull_ReturnsFalse(TodoTaskId task)
        {
            Assert.False(task.Equals(null));
        }
    }
}
