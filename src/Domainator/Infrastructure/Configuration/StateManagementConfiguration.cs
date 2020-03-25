using System;
using Domainator.Infrastructure.Repositories.StateManagement.Storage;
using Domainator.Utilities;

namespace Domainator.Infrastructure.Configuration
{
    public delegate object ObjectFactory(Type objectType);

    public delegate IAggregateStateStorage StateStorageFactory(ObjectFactory objectFactory);

    public sealed class StateManagementConfiguration
    {
        private readonly DomainatorInfrastructureBuilder _builder;
        private readonly Action<StateStorageFactory> _setFactory;

        public StateManagementConfiguration(DomainatorInfrastructureBuilder builder, Action<StateStorageFactory> setFactory)
        {
            Require.NotNull(builder, nameof(builder));
            Require.NotNull(setFactory, nameof(setFactory));

            _builder = builder;
            _setFactory = setFactory;
        }

        public IDomainatorInfrastructureBuilder UseStateStorageFactory(StateStorageFactory storageFactory)
        {
            Require.NotNull(storageFactory, nameof(storageFactory));

            _setFactory(storageFactory);

            return _builder;
        }
    }
}
