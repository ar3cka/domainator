using Domainator.Demo.Domain.Domain;
using Domainator.Demo.Domain.Infrastructure.Repositories.StateManagement.Serialization.Json;
using Domainator.DomainEvents;
using Domainator.Entities;
using Domainator.Infrastructure.Repositories.StateManagement;
using Domainator.Infrastructure.Repositories.StateManagement.Serialization.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Domainator.UnitTests.Infrastructure.Repositories.StateManagement.Serialization.Json
{
    public class AggregateStateJsonSerializerTests
    {
        private readonly AggregateStateJsonSerializer _serializer = new AggregateStateJsonSerializer(
            new JsonConverter[]
            {
                new UserIdJsonConverter()
            });

        [Theory]
        [AggregateStateJsonSerializerTestsData]
        public void SerializeStateTest(TodoTask.AggregateState state)
        {
            // act
            var serializedObject = JObject.Parse(_serializer.SerializeState(state));

            // assert
            Assert.Equal(state.ProjectId.Id, (int)serializedObject["projectId"]);
            Assert.Equal(TaskState.Created.ToString("G"), (string)serializedObject["taskState"]);
        }

        [Theory]
        [AggregateStateJsonSerializerTestsData]
        public void DeserializeStateTest(TodoTask.AggregateState state)
        {
            // arrange
            var serializedString = _serializer.SerializeState(state);

            // act
            var deserializedObject = _serializer.DeserializeState<TodoTask.AggregateState>(serializedString);

            // assert
            Assert.Equal(state.ProjectId.Id, deserializedObject.ProjectId.Id);
        }

        [Theory]
        [AggregateStateJsonSerializerTestsData]
        public void SerializeChangeSetTest(
            AggregateVersion version, TodoTaskCreated taskCreated, TodoTaskMoved taskMoved)
        {
            // arrange
            var changeSet = new ChangeSet(version, new IDomainEvent[] {taskCreated, taskMoved});

            // act
            var serializedChangeSet = JObject.Parse(_serializer.SerializeChangeSet(changeSet));

            // assert
            Assert.Equal((int)changeSet.FromVersion, serializedChangeSet["fromVersion"]);

            var serializedClrTypes = (JArray)serializedChangeSet["changeClrTypes"];
            Assert.Equal(typeof(TodoTaskCreated).AssemblyQualifiedName, serializedClrTypes[0]);
            Assert.Equal(typeof(TodoTaskMoved).AssemblyQualifiedName, serializedClrTypes[1]);

            var serializedChanges = (JArray)serializedChangeSet["changes"];
            var serializedTaskCreated = serializedChanges[0];
            Assert.Equal(taskCreated.TaskId.Id, (int)serializedTaskCreated["taskId"]);
            Assert.Equal(taskCreated.ProjectId.Id, (int)serializedTaskCreated["projectId"]);

            var serializedTaskMoved = serializedChanges[1];
            Assert.Equal(taskMoved.TaskId.Id, (int)serializedTaskMoved["taskId"]);
            Assert.Equal(taskMoved.NewProjectId.Id, (int)serializedTaskMoved["newProjectId"]);
            Assert.Equal(taskMoved.OldProjectId.Id, (int)serializedTaskMoved["oldProjectId"]);
        }

        [Theory]
        [AggregateStateJsonSerializerTestsData]
        public void DeserializeChangeSetTest(
            AggregateVersion version, TodoTaskCreated taskCreated, TodoTaskMoved taskMoved)
        {
            // arrange
            var changeSet = new ChangeSet(version, new IDomainEvent[] {taskCreated, taskMoved});
            var serializedChangeSet = _serializer.SerializeChangeSet(changeSet);

            // act
            var restoredChangeSet = _serializer.DeserializeChangeSet(serializedChangeSet);

            // assert
            Assert.Equal(changeSet.FromVersion, restoredChangeSet.FromVersion);
            Assert.Equal(changeSet.ToVersion, restoredChangeSet.ToVersion);

            var restoredTaskCreated = restoredChangeSet.Changes[0] as TodoTaskCreated;
            Assert.NotNull(restoredTaskCreated);
            Assert.Equal(taskCreated.TaskId, restoredTaskCreated.TaskId);
            Assert.Equal(taskCreated.ProjectId, restoredTaskCreated.ProjectId);

            var restoredTaskMoved = restoredChangeSet.Changes[1] as TodoTaskMoved;
            Assert.NotNull(restoredTaskMoved);
            Assert.Equal(taskMoved.TaskId, restoredTaskMoved.TaskId);
            Assert.Equal(taskMoved.NewProjectId, restoredTaskMoved.NewProjectId);
            Assert.Equal(taskMoved.OldProjectId, restoredTaskMoved.OldProjectId);
        }

        [Theory]
        [AggregateStateJsonSerializerTestsData]
        public void Serialize_UsesCustomSerializer(UserState state)
        {
            // act
            var serializedState = JObject.Parse(_serializer.SerializeState(state));

            // assert
            Assert.Equal(state.UserId.Id.ToString("N"), (string)serializedState["userId"]);
        }

        [Theory]
        [AggregateStateJsonSerializerTestsData]
        public void Deserialize_UsesCustomSerializer(UserState state)
        {
            // arrange
            var serializedState = _serializer.SerializeState(state);

            // act
            var restoredState = _serializer.DeserializeState<UserState>(serializedState);

            // assert
            Assert.Equal(state.UserId, restoredState.UserId);
        }
    }
}
