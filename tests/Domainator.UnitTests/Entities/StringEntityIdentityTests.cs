using System;
using AutoFixture.Xunit2;
using Domainator.Entities;
using Moq;
using Xunit;

namespace Domainator.UnitTests.Entities
{
    public class StringEntityIdentityTests
    {
        class TestIdentity : StringEntityIdentity
        {
            public override string Tag => "test_identity";

            public TestIdentity(IEntityIdentity identity) : base(identity)
            {
            }

            public TestIdentity(string id) : base(id)
            {
            }
        }

        [Theory]
        [AutoData]
        public void Constructor_FromTheValue(string id)
        {
            // arrange
            var taskId = new TestIdentity(id);

            // act && assert
            Assert.Equal(id, taskId.Id);
            Assert.Equal(id, taskId.Value);
        }

        [Theory]
        [AutoData]
        public void Constructor_FromIdentity(Mock<IEntityIdentity> identity, string id)
        {
            // arrange
            identity
                .SetupGet(self => self.Tag)
                .Returns("test_identity");

            identity
                .SetupGet(self => self.Value)
                .Returns(id.ToString);

            // act
            var taskId = new TestIdentity(identity.Object);

            // assert
            Assert.Equal(id, taskId.Id);
            Assert.Equal(id, taskId.Value);
        }

        [Theory]
        [AutoData]
        public void Constructor_WhenTagIsDifferent_ThrowsArgumentException(
            Mock<IEntityIdentity> identity, string tag, string value)
        {
            // arrange
            identity
                .SetupGet(self => self.Tag)
                .Returns(tag);

            identity
                .SetupGet(self => self.Value)
                .Returns(value);

            // act && assert
            Assert.Throws<ArgumentException>(() => new TestIdentity(identity.Object));
        }
    }
}
