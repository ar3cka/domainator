using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using Domainator.Demo.Domain.Domain;

namespace Domainator.Demo.UnitTests.Domain
{
    public sealed class TodoTaskTestsDataAttribute : AutoDataAttribute
    {
        public TodoTaskTestsDataAttribute() : base(CustomizeFixture)
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
