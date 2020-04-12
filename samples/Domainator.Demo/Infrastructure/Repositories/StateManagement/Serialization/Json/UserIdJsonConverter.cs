using System;
using System.Globalization;
using Domainator.Demo.Domain.Domain;
using Domainator.Utilities;
using Newtonsoft.Json;

namespace Domainator.Demo.Domain.Infrastructure.Repositories.StateManagement.Serialization.Json
{
    public class UserIdJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Require.NotNull(writer, nameof(writer));
            Require.NotNull(value, nameof(value));
            Require.NotNull(serializer, nameof(serializer));

            var userId = (UserId)value;

            serializer.Serialize(writer, userId.Id.ToString("N", CultureInfo.InvariantCulture));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Require.NotNull(reader, nameof(reader));
            Require.NotNull(serializer, nameof(serializer));

            var rawId = serializer.Deserialize<Guid>(reader);

            return new UserId(rawId);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UserId);
        }
    }
}
