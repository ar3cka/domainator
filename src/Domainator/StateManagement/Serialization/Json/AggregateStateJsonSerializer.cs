using Domainator.Entities;
using Domainator.Utilities;
using Newtonsoft.Json;

namespace Domainator.StateManagement.Serialization.Json
{
    /// <summary>
    /// Implements JSON <see cref="IAggregateStateSerializer"/> based on Newtonsoft.Json.
    /// </summary>
    public class AggregateStateJsonSerializer : IAggregateStateSerializer
    {
        private static readonly JsonConverter[] _defaultConverters =
        {
            new AbstractEntityIdentityValueConverter()
        };

        /// <inheritdoc />
        public string Serialize<TState>(TState state) where TState : class, IAggregateState
        {
            Require.NotNull(state, nameof(state));

            return JsonConvert.SerializeObject(state, _defaultConverters);
        }

        /// <inheritdoc />
        public TState Deserialize<TState>(string serializedState) where TState : class, IAggregateState
        {
            Require.NotEmpty(serializedState, nameof(serializedState));

            return JsonConvert.DeserializeObject<TState>(serializedState, _defaultConverters);
        }
    }
}
