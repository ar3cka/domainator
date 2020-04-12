using System;
using System.Linq;
using Domainator.Infrastructure.Configuration;
using Newtonsoft.Json;
using Xunit;

namespace Domainator.UnitTests.Infrastructure.Configuration
{
    public class DomainatorInfrastructureBuilderTests
    {
        public class Dummy
        {
            public Guid  Value { get; set; }
        }

        public class DummyJsonConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteValue(((Dummy)value).Value.ToString("N"));
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return new Dummy
                {
                    Value = Guid.Parse(reader.ReadAsString())
                };
            }

            public override bool CanConvert(Type objectType) => throw new NotImplementedException();
        }

        [Fact]
        public void UseJsonConverter_AddsConverterTypeToCustomJsonConvertersCollection()
        {
            var builder = new DomainatorInfrastructureBuilder();

            builder.Serialization.UseJsonConverter<DummyJsonConverter>();

            Assert.Contains(builder.CustomJsonConverters, type => type == typeof(DummyJsonConverter));
        }
    }
}
