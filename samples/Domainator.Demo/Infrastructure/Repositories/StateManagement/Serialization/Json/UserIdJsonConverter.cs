using System;
using Domainator.Demo.Domain.Domain;
using Newtonsoft.Json;

namespace Domainator.Demo.Domain.Infrastructure.Repositories.StateManagement.Serialization.Json
{
    public class UserIdJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var userId = (UserId)value;

            writer.WriteValue(userId.Id.ToString("N"));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var rawId = Guid.Parse(reader.ReadAsString());

            return new UserId(rawId);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UserId);
        }
    }
}
