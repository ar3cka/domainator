using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Domainator.Entities;
using Domainator.Utilities;
using Newtonsoft.Json;

namespace Domainator.Infrastructure.Repositories.StateManagement.Serialization.Json
{
    internal class AbstractEntityIdentityValueConverter : JsonConverter
    {
        private static readonly ConcurrentDictionary<Type, IdentityTypeConverterImpl> _converters = new ConcurrentDictionary<Type,IdentityTypeConverterImpl>();

        private class IdentityTypeConverterImpl
        {
            private readonly PropertyInfo _idProperty;
            private readonly ConstructorInfo _ctor;

            public IdentityTypeConverterImpl(Type identityType)
            {
                _idProperty = identityType.GetProperty("Id", BindingFlags.Instance | BindingFlags.Public);

                Debug.Assert(_idProperty != null, nameof(_idProperty) + " != null");

                _ctor = identityType.GetConstructor(new [] { _idProperty.PropertyType });
            }

            public object ReadJson(JsonReader reader, JsonSerializer serializer)
            {
                var idValue = serializer.Deserialize(reader, _idProperty.PropertyType);
                return _ctor.Invoke(new [] { idValue });
            }
        }

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
