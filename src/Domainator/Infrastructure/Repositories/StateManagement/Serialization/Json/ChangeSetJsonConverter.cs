using System;
using Domainator.DomainEvents;
using Domainator.Entities;
using Domainator.Utilities;
using Newtonsoft.Json;

namespace Domainator.Infrastructure.Repositories.StateManagement.Serialization.Json
{
    internal sealed class ChangeSetJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var changeSet = (ChangeSet)value;

            writer.WriteStartObject();

            writer.WritePropertyName("fromVersion");
            writer.WriteValue((int)changeSet.FromVersion);

            writer.WritePropertyName("changeClrTypes");
            writer.WriteStartArray();
            foreach (var domainEvent in changeSet.Changes)
            {
                writer.WriteValue(domainEvent.GetType().AssemblyQualifiedName);
            }
            writer.WriteEndArray();

            writer.WritePropertyName("changes");
            writer.WriteStartArray();
            foreach (var domainEvent in changeSet.Changes)
            {
                serializer.Serialize(writer, domainEvent);
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            reader.Read();
            Ensure.True(reader.TokenType == JsonToken.PropertyName, "reader.TokenType == JsonToken.PropertyName");
            Ensure.True(reader.Path == "fromVersion", "reader.Path == \"fromVersion\"");
            var fromVersion = reader.ReadAsInt32() ?? throw new InvalidOperationException("Failed to read \"fromVersion\" property value.");

            reader.Read();
            Ensure.True(reader.TokenType == JsonToken.PropertyName, "reader.TokenType == JsonToken.PropertyName");
            Ensure.True(reader.Path == "changeClrTypes", "reader.Path == \"changeClrTypes\"");
            reader.Read();
            Ensure.True(reader.TokenType == JsonToken.StartArray, "reader.TokenType == JsonToken.StartArray");
            var clrTypes = serializer.Deserialize<string[]>(reader);

            reader.Read();
            Ensure.True(reader.TokenType == JsonToken.PropertyName, "reader.TokenType == JsonToken.PropertyName");
            Ensure.True(reader.Path == "changes", "reader.Path == \"changes\"");

            reader.Read();
            Ensure.True(reader.TokenType == JsonToken.StartArray, "reader.TokenType == JsonToken.StartArray");
            var events = new IDomainEvent[clrTypes.Length];
            for (var i = 0; i < clrTypes.Length; i++)
            {
                reader.Read();
                Ensure.True(reader.TokenType == JsonToken.StartObject, "reader.TokenType == JsonToken.StartObject");

                events[i] = (IDomainEvent)serializer.Deserialize(reader, Type.GetType(clrTypes[i], true));
            }

            reader.Read(); // Move from EndObject
            reader.Read(); // Move from EndArray

            return new ChangeSet(AggregateVersion.Create(fromVersion), events);
        }

        public override bool CanConvert(Type objectType) => objectType == typeof(ChangeSet);
    }
}
