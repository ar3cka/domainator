using System;
using Domainator.Demo.Domain.Domain;
using Domainator.Entities;
using Moq;
using Xunit;

namespace Domainator.UnitTests.Entities
{
    public class Int32EntityIdentityTests
    {
        [Theory]
        [AbstractEntityIdentityTestsData]
        public void Constructor_FromTheValue(int id)
        {
            // arrange
            var taskId = new TodoTaskId(id);

            // act && assert
            Assert.Equal(id, taskId.Id);
            Assert.Equal(id.ToString(), taskId.Value);
        }

        [Theory]
        [AbstractEntityIdentityTestsData]
        public void Constructor_FromIdentity(Mock<IEntityIdentity> identity, int id)
        {
            // arrange
            identity
                .SetupGet(self => self.Tag)
                .Returns("todo_task");

            identity
                .SetupGet(self => self.Value)
                .Returns(id.ToString);

            // act
            var taskId = new TodoTaskId(identity.Object);

            // assert
            Assert.Equal(id, taskId.Id);
            Assert.Equal(id.ToString(), taskId.Value);
        }

        [Theory]
        [AbstractEntityIdentityTestsData]
        public void Constructor_WhenTagIsDifferent_ThrowsArgumentException(Mock<IEntityIdentity> identity)
        {
            // act && assert
            Assert.Throws<ArgumentException>(() => new TodoTaskId(identity.Object));
        }
    }
}
