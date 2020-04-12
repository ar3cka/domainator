using System;
using Domainator.Utilities;
using Newtonsoft.Json;

namespace Domainator.Infrastructure.Configuration
{
    public sealed class SerializationConfiguration
    {
        private readonly DomainatorInfrastructureBuilder _builder;
        private readonly Action<Type> _registerConverterType;

        public SerializationConfiguration(DomainatorInfrastructureBuilder builder, Action<Type> registerConverterType)
        {
            Require.NotNull(builder, nameof(builder));
            Require.NotNull(registerConverterType, nameof(registerConverterType));

            _builder = builder;
            _registerConverterType = registerConverterType;
        }

        public IDomainatorInfrastructureBuilder UseJsonConverter<TConverter>() where TConverter : JsonConverter
        {
            _registerConverterType(typeof(TConverter));

            return _builder;
        }
    }
}
