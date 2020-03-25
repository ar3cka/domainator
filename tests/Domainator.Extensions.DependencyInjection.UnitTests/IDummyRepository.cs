using Domainator.Demo.Domain.Domain;
using Domainator.Entities;

namespace Domainator.Extensions.DependencyInjection.UnitTests
{
    public interface IDummyRepository : IRepository<TodoTaskId, TodoTask>
    {
    }
}
