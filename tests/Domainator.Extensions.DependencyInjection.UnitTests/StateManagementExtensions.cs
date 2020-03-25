using Domainator.Infrastructure.Configuration;

namespace Domainator.Extensions.DependencyInjection.UnitTests
{
    public static class StateManagementExtensions
    {
        public static IDomainatorInfrastructureBuilder UseDummyStorage(this StateManagementConfiguration configuration)
        {
            return configuration.UseStateStorageFactory(provider => new DummyStateStorage());
        }
    }
}
