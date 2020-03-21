using AutoFixture.Xunit2;
using Domainator.Demo.Domain.Domain;
using Domainator.Infrastructure.StateManagement.Serialization.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Domainator.UnitTests.StateManagement.Serialization.Json
{
    public class AggregateStateJsonSerializerTests
    {
        private readonly AggregateStateJsonSerializer _serializer = new AggregateStateJsonSerializer();

        [Theory]
        [AutoData]
        public void SerializeObject_UsesIdPropertyValue(TodoTaskState state)
        {
            // act
            var serializedObject = JObject.Parse(_serializer.Serialize(state));

            // assert
            Assert.Equal(state.ProjectId.Id, (int)serializedObject["projectId"]);
            Assert.Equal(TaskState.Created.ToString("G"), (string)serializedObject["taskState"]);
        }

        [Theory]
        [AutoData]
        public void DeserializeObject_RestoresIdentityFromTheIdValue(TodoTaskState state)
        {
            // arrange
            var serializedString = _serializer.Serialize(state);

            // act
            var deserializedObject = _serializer.Deserialize<TodoTaskState>(serializedString);

            // assert
            Assert.Equal(state.ProjectId, deserializedObject.ProjectId);
        }
    }
}
