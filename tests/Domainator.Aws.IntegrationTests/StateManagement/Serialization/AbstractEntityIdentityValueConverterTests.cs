using AutoFixture.Xunit2;
using Domainator.Demo.Domain.Domain;
using Domainator.Infrastructure.StateManagement.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Domainator.Aws.IntegrationTests.StateManagement.Serialization
{
    public class AbstractEntityIdentityValueConverterTests
    {
        [Theory]
        [AutoData]
        public void SerializeObject_UsesIdPropertyValue(TodoTaskState state)
        {
            // arrange
            var converter = new AbstractEntityIdentityValueConverter();

            // act
            var serializedObject = JObject.Parse(JsonConvert.SerializeObject(state, converter));

            // assert
            Assert.Equal(state.ProjectId.Id, (int)serializedObject["ProjectId"]);
        }

        [Theory]
        [AutoData]
        public void DeserializeObject_RestoresIdentityFromTheIdValue(TodoTaskState state)
        {
            // arrange
            var converter = new AbstractEntityIdentityValueConverter();
            var serializedString = JsonConvert.SerializeObject(state, converter);

            // act
            var deserializedObject = JsonConvert.DeserializeObject<TodoTaskState>(serializedString, converter);

            // assert
            Assert.Equal(state.ProjectId, deserializedObject.ProjectId);
        }
    }
}