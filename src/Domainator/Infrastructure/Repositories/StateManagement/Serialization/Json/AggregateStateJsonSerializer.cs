using Domainator.Entities;
using Domainator.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Domainator.Infrastructure.Repositories.StateManagement.Serialization.Json
{
    /// <summary>
    /// Implements JSON <see cref="IAggregateStateSerializer"/> based on Newtonsoft.Json.
    /// </summary>
    public class AggregateStateJsonSerializer : IAggregateStateSerializer
    {
        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            },
            Converters =
            {
                new AbstractEntityIdentityValueConverter(),
                new ChangeSetJsonConverter(),
                new StringEnumConverter()
            },
            Formatting = Formatting.None
        };

        /// <inheritdoc />
        public string SerializeState<TState>(TState state) where TState : class, IAggregateState
        {
            Require.NotNull(state, nameof(state));

            return JsonConvert.SerializeObject(state, _jsonSerializerSettings);
        }

        /// <inheritdoc />
        public TState DeserializeState<TState>(string serializedState) where TState : class, IAggregateState
        {
            Require.NotEmpty(serializedState, nameof(serializedState));

            return JsonConvert.DeserializeObject<TState>(serializedState, _jsonSerializerSettings);
        }

        public string SerializeChangeSet(ChangeSet changeSet)
        {
            Require.NotNull(changeSet, nameof(changeSet));

            return JsonConvert.SerializeObject(changeSet, _jsonSerializerSettings);
        }

        public ChangeSet DeserializeChangeSet(string serializedChangeSet)
        {
            Require.NotEmpty(serializedChangeSet, nameof(serializedChangeSet));

            return JsonConvert.DeserializeObject<ChangeSet>(serializedChangeSet, _jsonSerializerSettings);
        }
    }
}
