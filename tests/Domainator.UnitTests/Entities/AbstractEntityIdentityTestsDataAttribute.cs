using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using Domainator.Demo.Domain.Domain;

namespace Domainator.UnitTests.Entities
{
    public sealed class AbstractEntityIdentityTestsDataAttribute : AutoDataAttribute
    {
        public AbstractEntityIdentityTestsDataAttribute() : base(CustomizeFixture)
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
