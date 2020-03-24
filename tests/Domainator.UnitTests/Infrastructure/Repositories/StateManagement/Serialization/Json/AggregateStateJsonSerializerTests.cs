using AutoFixture.Xunit2;
using Domainator.Demo.Domain.Domain;
using Domainator.Infrastructure.Repositories.StateManagement.Serialization.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Domainator.UnitTests.Infrastructure.Repositories.StateManagement.Serialization.Json
{
    public class AggregateStateJsonSerializerTests
    {
        private readonly AggregateStateJsonSerializer _serializer = new AggregateStateJsonSerializer();

        [Theory]
        [AutoData]
        public void SerializeObject_UsesIdPropertyValue(TodoTask.AggregateState state)
        {
            // act
            var serializedObject = JObject.Parse(_serializer.Serialize(state));

            // assert
            Assert.Equal(state.ProjectId.Id, (int)serializedObject["projectId"]);
            Assert.Equal(TaskState.Created.ToString("G"), (string)serializedObject["taskState"]);
        }

        [Theory]
        [AutoData]
        public void DeserializeObject_RestoresIdentityFromTheIdValue(TodoTask.AggregateState state)
        {
            // arrange
            var serializedString = _serializer.Serialize(state);

            // act
            var deserializedObject = _serializer.Deserialize<TodoTask.AggregateState>(serializedString);

            // assert
            Assert.Equal(state.ProjectId, deserializedObject.ProjectId);
        }
    }
}
