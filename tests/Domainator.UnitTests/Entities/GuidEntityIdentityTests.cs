using System;
using Domainator.Demo.Domain.Domain;
using Domainator.Entities;
using Moq;
using Xunit;

namespace Domainator.UnitTests.Entities
{
    public class GuidEntityIdentityTests
    {
        [Theory]
        [AbstractEntityIdentityTestsData]
        public void Constructor_FromTheValue(Guid id)
        {
            // arrange
            var userId = new UserId(id);

            // act && assert
            Assert.Equal(id, userId.Id);
            Assert.Equal(id.ToString(), userId.Value);
        }

        [Theory]
        [AbstractEntityIdentityTestsData]
        public void Constructor_FromIdentity(Mock<IEntityIdentity> identity, Guid id)
        {
            // arrange
            identity
                .SetupGet(self => self.Tag)
                .Returns("user");

            identity
                .SetupGet(self => self.Value)
                .Returns(id.ToString);

            // act
            var userId = new UserId(identity.Object);

            // assert
            Assert.Equal(id, userId.Id);
            Assert.Equal(id.ToString(), userId.Value);
        }

        [Theory]
        [AbstractEntityIdentityTestsData]
        public void Constructor_WhenTagIsDifferent_ThrowsArgumentException(Mock<IEntityIdentity> identity, int id)
        {
            // act && assert
            Assert.Throws<ArgumentException>(() => new ProjectId(identity.Object));
        }
    }
}
