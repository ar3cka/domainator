using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using Domainator.Demo.Domain.Domain;

namespace Domainator.Aws.IntegrationTests.Repositories.StateManagement.Storage
{
    public sealed class DynamoDbAggregateStateStorageTestsDataAttribute : AutoDataAttribute
    {
        public DynamoDbAggregateStateStorageTestsDataAttribute() : base(CustomizeFixture)
        {
        }

        private static IFixture CustomizeFixture()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});

            fixture.Register((int id) => new TodoTaskId(id));
            fixture.Register((int id) => new ProjectId(id));

            return fixture;
        }
    }
}
