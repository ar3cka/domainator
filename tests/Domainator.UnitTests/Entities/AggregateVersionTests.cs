using AutoFixture.Xunit2;
using Domainator.Entities;
using Xunit;

namespace Domainator.UnitTests.Entities
{
    public class AggregateVersionTests
    {
        [Fact]
        public void Parse_ForZero_ReturnsUnknownVersion()
        {
            var version = AggregateVersion.Parse("0");

            Assert.Equal(AggregateVersion.Emtpy, version);
        }

        [Fact]
        public void IsUnknown_ForZeroVersion_ReturnsTrue()
        {
            var version = AggregateVersion.Parse("0");

            Assert.True(AggregateVersion.IsEmpty(version));
        }

        [Theory]
        [AutoData]
        public void Increment_IncreasesVersion(int versionValue, int incrementValue)
        {
            var expectedVersion = AggregateVersion.Create(versionValue + incrementValue);
            var version = AggregateVersion.Create(versionValue);

            var incrementedVersion = version.Increment(incrementValue);

            Assert.Equal(expectedVersion, incrementedVersion);
        }

        [Theory]
        [AutoData]
        public void Increment_IncreasesVersionByOne(int versionValue)
        {
            var expectedVersion = AggregateVersion.Create(versionValue + 1);
            var version = AggregateVersion.Create(versionValue);

            var incrementedVersion = version.Increment();

            Assert.Equal(expectedVersion, incrementedVersion);
        }

        [Theory]
        [AutoData]
        public void Equals_ForTheSameVersion_ReturnsTrue(AggregateVersion version)
        {
            Assert.Equal(version, version);
        }

        [Theory]
        [AutoData]
        public void ComparisonTests(AggregateVersion version, int increment)
        {
            var incrementedVersion = version.Increment(increment);
            var copiedOriginalVersion = AggregateVersion.Create((int)version);

            Assert.True(version == copiedOriginalVersion);
            Assert.True(version <= copiedOriginalVersion);

            Assert.True(version < incrementedVersion);
            Assert.True(version <= incrementedVersion);

            Assert.True(incrementedVersion > version);
            Assert.True(incrementedVersion >= version);
        }
    }
}
