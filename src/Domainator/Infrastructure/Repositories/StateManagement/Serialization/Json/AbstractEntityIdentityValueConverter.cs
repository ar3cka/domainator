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
        private static readonly ConcurrentDictionary<Type, bool> _supportedTypes = new ConcurrentDictionary<Type, bool>();

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

            public void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var idValue = _idProperty.GetValue(value);

                serializer.Serialize(writer, idValue);
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

            var converter = _converters.GetOrAdd(value.GetType(), t => new IdentityTypeConverterImpl(t));
            converter.WriteJson(writer, value, serializer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Require.NotNull(reader, nameof(reader));
            Require.NotNull(objectType, nameof(objectType));
            Require.NotNull(serializer, nameof(serializer));

            var converter = _converters.GetOrAdd(objectType, t => new IdentityTypeConverterImpl(t));
            return converter.ReadJson(reader, serializer);
        }

        public override bool CanConvert(Type objectType)
        {
            Require.NotNull(objectType, nameof(objectType));

            return _supportedTypes.GetOrAdd(objectType, CanConvertInternal);
        }

        private static bool CanConvertInternal(Type objectType)
        {
            if (typeof(IEntityIdentity).IsAssignableFrom(objectType))
            {
                var baseType = objectType.BaseType;
                while (baseType != null)
                {
                    if (baseType.IsConstructedGenericType)
                    {
                        var genericTypeDefinition = baseType.GetGenericTypeDefinition();
                        return genericTypeDefinition == typeof(AbstractEntityIdentity<>);
                    }

                    baseType = baseType.BaseType;
                }
            }

            return false;
        }
    }
}
