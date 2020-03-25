using Domainator.Demo.Domain.Domain;
using Domainator.Entities;
using Domainator.Infrastructure.DependencyInjection;
using Domainator.Infrastructure.Repositories;
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
            // act
            _services.AddDomainatorInfrastructure(builder =>
            {
                builder.StateManagement.UseDummyStorage();
                builder.Repository.ForAggregate<TodoTaskId, TodoTask, TodoTask.AggregateState>();
            });

            // assert
            var provider = _services.BuildServiceProvider();

            // assert
            var repository = provider.GetService<IRepository<TodoTaskId, TodoTask>>();
            Assert.NotNull(repository);
            Assert.IsType<GenericRepository<TodoTaskId, TodoTask, TodoTask.AggregateState>>(repository);
        }

        [Fact]
        public void AddDomainatorInfrastructure_WhenRepositoryTypeSpecified_RegistersInterfaceAndImplementation()
        {
            // act
            _services.AddDomainatorInfrastructure(builder =>
            {
                builder.StateManagement.UseDummyStorage();

                builder.Repository.ForAggregate<TodoTaskId, TodoTask>(repositoryBuilder =>
                {
                    repositoryBuilder.UseRepository<IDummyRepository, DummyRepository>();
                });
            });

            // assert
            var provider = _services.BuildServiceProvider();

            // assert
            var repository = provider.GetService<IDummyRepository>();
            Assert.NotNull(repository);
            Assert.IsType<DummyRepository>(repository);
            Assert.IsType<DummyStateStorage>(((DummyRepository)repository).StateStorage);
        }
    }
}
