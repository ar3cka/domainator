using System;
using Domainator.Entities;
using Domainator.Utilities;
using Newtonsoft.Json;

namespace Domainator.Infrastructure.Repositories.StateManagement.Serialization.Json
{
    internal class AbstractEntityIdentityValueConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Require.NotNull(writer, nameof(writer));
            Require.NotNull(value, nameof(value));
            Require.NotNull(serializer, nameof(serializer));

            var rawValue = AbstractEntityIdentity.ExtractRawValue(value);
            serializer.Serialize(writer, rawValue);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Require.NotNull(reader, nameof(reader));
            Require.NotNull(objectType, nameof(objectType));
            Require.NotNull(serializer, nameof(serializer));

            var rawValueType = AbstractEntityIdentity.GetRawType(objectType);
            var rawValue = serializer.Deserialize(reader, rawValueType);

            return AbstractEntityIdentity.CreateFromRawValue(rawValue, objectType);
        }

        public override bool CanConvert(Type objectType)
        {
            Require.NotNull(objectType, nameof(objectType));

            return AbstractEntityIdentity.IsValidType(objectType);
        }
    }
}
