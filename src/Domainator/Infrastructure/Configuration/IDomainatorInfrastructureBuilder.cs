namespace Domainator.Infrastructure.Configuration
{
    /// <summary>
    /// Infrastructure builder interface
    /// </summary>
    public interface IDomainatorInfrastructureBuilder
    {
        /// <summary>
        /// Gets configuration of Aggregate state management infrastructure
        /// </summary>
        StateManagementConfiguration StateManagement { get; }

        /// <summary>
        /// Gets configuration of Aggregate root repositories
        /// </summary>
        RepositoriesConfiguration Repository { get; }

        /// <summary>
        /// Gets serialization configuration
        /// </summary>
        SerializationConfiguration Serialization { get; }
    }
}
