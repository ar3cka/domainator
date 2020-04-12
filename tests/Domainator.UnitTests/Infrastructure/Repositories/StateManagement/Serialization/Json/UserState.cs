using Domainator.Demo.Domain.Domain;
using Domainator.Entities;

namespace Domainator.UnitTests.Infrastructure.Repositories.StateManagement.Serialization.Json
{
    public class UserState : AbstractAggregateState
    {
        public UserId UserId { get; set; }
    }
}
