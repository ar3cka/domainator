using Domainator.Entities;
using Domainator.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Domainator.Infrastructure.StateManagement.Serialization.Json
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
                new StringEnumConverter()
            }
        };

        /// <inheritdoc />
        public string Serialize<TState>(TState state) where TState : class, IAggregateState
        {
            Require.NotNull(state, nameof(state));

            return JsonConvert.SerializeObject(state, _jsonSerializerSettings);
        }

        /// <inheritdoc />
        public TState Deserialize<TState>(string serializedState) where TState : class, IAggregateState
        {
            Require.NotEmpty(serializedState, nameof(serializedState));

            return JsonConvert.DeserializeObject<TState>(serializedState, _jsonSerializerSettings);
        }
    }
}
