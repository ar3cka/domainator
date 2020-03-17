using AutoFixture.Xunit2;
using Domainator.Demo.Domain.Domain;
using Domainator.Entities;
using Xunit;

namespace Domainator.UnitTests.Entities
{
    public class AbstractAggregateStateTests
    {
        [Theory]
        [AutoData]
        public void NewRoot_HasDefaultStateAndVersion(TestableAbstractAggregateRoot entity)
        {
            // assert
            Assert.NotNull(entity.State);
            Assert.True(AggregateVersion.IsEmpty(entity.Version));
        }

        [Theory]
        [AutoData]
        public void RestoreFromState_UpdatesStateAndVersionOfTheAggregate(TestableAbstractAggregateRoot entity, TodoTaskState newState, AggregateVersion newVersion)
        {
            // act
            entity.RestoreFromState(newState, newVersion);

            // assert
            Assert.Same(newState, entity.State);
            Assert.Equal(newVersion, entity.Version);
        }
    }
}

