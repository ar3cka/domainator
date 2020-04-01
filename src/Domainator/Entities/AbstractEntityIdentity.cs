using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Domainator.Utilities;

namespace Domainator.Entities
{
    /// <summary>
    /// Companion class of <see cref="AbstractEntityIdentity{TKey}"/> with utilities methods.
    /// </summary>
    public static class AbstractEntityIdentity
    {
        private static readonly ConcurrentDictionary<Type, RawValueHandler> _identityHandlers = new ConcurrentDictionary<Type, RawValueHandler>();
        private static readonly ConcurrentDictionary<Type, bool> _identityTypes = new ConcurrentDictionary<Type, bool>();

        private class RawValueHandler
        {
            private readonly PropertyInfo _idProperty;
            private readonly ConstructorInfo _ctor;

            public RawValueHandler(Type identityType)
            {
                _idProperty = identityType.GetProperty("Id", BindingFlags.Instance | BindingFlags.Public);

                Debug.Assert(_idProperty != null, nameof(_idProperty) + " != null");

                _ctor = identityType.GetConstructor(new [] { _idProperty.PropertyType });
            }

            public object ExtractRawValue(object idValue)
            {
                return _idProperty.GetValue(idValue);
            }

            public object CreateFromRawValue(object rawValue)
            {
                return _ctor.Invoke(new [] { rawValue });
            }

            public Type GetRawValueType()
            {
                return _idProperty.PropertyType;
            }
        }

        public static Type GetRawType(Type idType)
        {
            Require.NotNull(idType, nameof(idType));
            Require.True(IsValidType(idType), nameof(idType), "Invalid identity type");

            var handler = _identityHandlers.GetOrAdd(idType, t => new RawValueHandler(t));

            return handler.GetRawValueType();
        }

        /// <summary>
        /// Extracts raw value from an instance of <see cref="AbstractEntityIdentity{TKey}"/>.
        /// </summary>
        /// <param name="idValue">The instance of identity.</param>
        public static object ExtractRawValue(object idValue)
        {
            Require.NotNull(idValue, nameof(idValue));
            Require.True(IsValidType(idValue.GetType()), nameof(idValue), "Invalid identity type");

            var handler = _identityHandlers.GetOrAdd(idValue.GetType(), t => new RawValueHandler(t));

            return handler.ExtractRawValue(idValue);
        }

        /// <summary>
        /// Checks whether <paramref name="objectType"/> is an implementation of <see cref="AbstractEntityIdentity{TKey}"/>.
        /// </summary>
        /// <param name="objectType">The type of checking.</param>
        public static bool IsValidType(Type objectType)
        {
            Require.NotNull(objectType, nameof(objectType));

            return _identityTypes.GetOrAdd(objectType, type =>
            {
                if (typeof(IEntityIdentity).IsAssignableFrom(type))
                {
                    var baseType = type.BaseType;
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
            });
        }

        /// <summary>
        /// Creates an instance of <see cref="AbstractEntityIdentity"/> from a raw value.
        /// </summary>
        /// <param name="rawValue">The raw value of identity.</param>
        /// <param name="idType">The type of identity.</param>
        public static object CreateFromRawValue(object rawValue, Type idType)
        {
            Require.NotNull(rawValue, nameof(rawValue));
            Require.True(IsValidType(idType), nameof(idType), "Invalid identity type");

            var handler = _identityHandlers.GetOrAdd(idType, t => new RawValueHandler(t));

            return handler.CreateFromRawValue(rawValue);
        }
    }
}
