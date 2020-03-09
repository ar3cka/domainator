using System;
using AutoFixture.Xunit2;
using Domainator.Entities;
using Xunit;

namespace Domainator.UnitTests.Entities
{
    public class AbstractEntityIdentityTests
    {
        public sealed class TaskId : AbstractEntityIdentity<int>
        {
            public override string Tag => "task";

            public TaskId(int id)
            {
                Id = id;
            }

            public override int Id { get; protected set; }
        }

        public sealed class UserId : AbstractEntityIdentity<int>
        {
            public override string Tag => "user";

            public UserId(int id)
            {
                Id = id;
            }

            public override int Id { get; protected set; }
        }

        [Theory]
        [AutoData]
        public void EqualityTests_EqualsForTheSameTaskReturnsTrue(TaskId taskId)
        {
            Assert.Equal(taskId, taskId);
            Assert.True(taskId == new TaskId(taskId.Id));
            Assert.True(taskId.Equals(new TaskId(taskId.Id)));
        }

        [Theory]
        [AutoData]
        public void EqualityTests_EqualsForDifferentTasksReturnsFalse(TaskId task1, TaskId task2)
        {
            Assert.NotEqual(task1, task2);
            Assert.False(task1 == task2);
            Assert.False(task1.Equals(task2));
        }

        [Theory]
        [AutoData]
        public void EqualityTests_EqualsForDifferentAbstractEntityIdentitySubTypesReturnsFalse(TaskId task, UserId user)
        {
            var taskIdentity = task as AbstractEntityIdentity<int>;
            var userIdentity = user as AbstractEntityIdentity<int>;

            Assert.NotEqual(taskIdentity, userIdentity);
            Assert.False(taskIdentity == userIdentity);
            Assert.False(taskIdentity.Equals(userIdentity));
        }

        [Theory]
        [AutoData]
        public void EqualityTests_EqualsForDifferentEntityIdentityTypesReturnsFalse(TaskId task, UserId user)
        {
            var taskIdentity = task as IEntityIdentity;
            var userIdentity = user as IEntityIdentity;

            Assert.NotEqual(taskIdentity, userIdentity);
            Assert.False(taskIdentity.Equals(userIdentity));
        }
    }
}
