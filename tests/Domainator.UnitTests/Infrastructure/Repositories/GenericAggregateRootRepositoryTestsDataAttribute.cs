using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using Domainator.Demo.Domain.Domain;

namespace Domainator.UnitTests.Infrastructure.Repositories
{
    public sealed class GenericAggregateRootRepositoryTestsDataAttribute : AutoDataAttribute
    {
        public GenericAggregateRootRepositoryTestsDataAttribute() : base(CustomizeFixture)
        {
        }

        private static IFixture CustomizeFixture()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});

            fixture.Customize<TodoTask>(c => c.FromFactory(
                new MethodInvoker(new GreedyConstructorQuery())));

            return fixture;
        }
    }
}
