using Domainator.Demo.Domain.Domain;
using Domainator.Demo.Domain.Infrastructure.Repositories.StateManagement.Serialization.Json;
using Domainator.Entities;
using Domainator.Infrastructure.DependencyInjection;
using Domainator.Infrastructure.Repositories;
using Domainator.Infrastructure.Repositories.StateManagement.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Domainator.Extensions.DependencyInjection.UnitTests
{
    public class ServiceCollectionExtensionsTests
    {
        private readonly ServiceCollection _services;

        public ServiceCollectionExtensionsTests()
        {
            _services = new ServiceCollection();
        }

        [Fact]
        public void AddDomainatorInfrastructure_WhenNoRepositoryTypeSpecified_RegistersGenericImplementation()
        {
            // arrange
            _services.AddDomainatorInfrastructure(builder =>
            {
                builder.StateManagement.UseDummyStorage();
                builder.Repository.ForAggregate<TodoTaskId, TodoTask, TodoTask.AggregateState>();
            });

            // act
            var provider = _services.BuildServiceProvider();

            // assert
            var repository = provider.GetService<IRepository<TodoTaskId, TodoTask>>();
            Assert.NotNull(repository);
            Assert.IsType<GenericRepository<TodoTaskId, TodoTask, TodoTask.AggregateState>>(repository);
        }

        [Fact]
        public void AddDomainatorInfrastructure_WhenRepositoryTypeSpecified_RegistersInterfaceAndImplementation()
        {
            // arrange
            _services.AddDomainatorInfrastructure(builder =>
            {
                builder.StateManagement.UseDummyStorage();

                builder.Repository.ForAggregate<TodoTaskId, TodoTask>(repositoryBuilder =>
                {
                    repositoryBuilder.UseRepository<IDummyRepository, DummyRepository>();
                });
            });

            // act
            var provider = _services.BuildServiceProvider();

            // assert
            var repository = provider.GetService<IDummyRepository>();
            Assert.NotNull(repository);
            Assert.IsType<DummyRepository>(repository);
            Assert.IsType<DummyStateStorage>(((DummyRepository)repository).StateStorage);
        }

        [Fact]
        public void AddDomainatorInfrastructure_WhenCustomJsonConverterSpecified_CanResolveSerializer()
        {
            // arrange
            _services.AddDomainatorInfrastructure(builder =>
            {
                builder.StateManagement.UseDummyStorage();
                builder.Serialization.UseJsonConverter<UserIdJsonConverter>();
            });

            // act
            var provider = _services.BuildServiceProvider();
            var serializer = provider.GetService<IAggregateStateSerializer>();

            // assert
            Assert.NotNull(serializer);
        }
    }
}
