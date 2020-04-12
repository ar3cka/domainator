using System;
using System.Collections.Generic;

namespace Domainator.Infrastructure.Configuration
{
    /// <inheritdoc />
    public class DomainatorInfrastructureBuilder : IDomainatorInfrastructureBuilder
    {
        private readonly List<Type> _customJsonConverters = new List<Type>();
        private readonly List<RepositoryDescriptor> _repositories = new List<RepositoryDescriptor>();
        private StateStorageFactory _stateStorageFactory;

        /// <inheritdoc />
        public StateManagementConfiguration StateManagement => new StateManagementConfiguration(this, stateFactory => _stateStorageFactory = stateFactory);

        /// <inheritdoc />
        public RepositoriesConfiguration Repository => new RepositoriesConfiguration(this, desc => _repositories.Add(desc));

        public SerializationConfiguration Serialization => new SerializationConfiguration(this, converter => _customJsonConverters.Add(converter));

        /// <summary>
        /// Gets configured state storage factory
        /// </summary>
        public StateStorageFactory StateStorageFactory => _stateStorageFactory;

        /// <summary>
        /// Gets the list if configured repositories
        /// </summary>
        public IEnumerable<RepositoryDescriptor> RegisteredRepositories => _repositories;

        /// <summary>
        /// Gets the list of configured custom converters
        /// </summary>
        public IReadOnlyCollection<Type> CustomJsonConverters => _customJsonConverters;
    }
}
