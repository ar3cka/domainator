using Domainator.Demo.Domain.Infrastructure.Repositories.StateManagement.Serialization.Json;
using Domainator.Infrastructure.Configuration;
using Xunit;

namespace Domainator.UnitTests.Infrastructure.Configuration
{
    public class DomainatorInfrastructureBuilderTests
    {
        [Fact]
        public void UseJsonConverter_AddsConverterTypeToCustomJsonConvertersCollection()
        {
            var builder = new DomainatorInfrastructureBuilder();

            builder.Serialization.UseJsonConverter<UserIdJsonConverter>();

            Assert.Contains(builder.CustomJsonConverters, type => type == typeof(UserIdJsonConverter));
        }
    }
}
