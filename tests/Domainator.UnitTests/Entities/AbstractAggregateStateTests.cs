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
        public void RestoreFromState_UpdatesStateAndVersionOfTheAggregate(
            [Frozen] AggregateVersion version,
            [Frozen] TodoTask.AggregateState state,
            [Frozen] TodoTaskId id,
            TestableAbstractAggregateRoot entity)
        {
            // assert
            Assert.Equal(id, entity.Id);
            Assert.Equal(version, entity.Version);
            Assert.Same(state, entity.State);
        }
    }
}

