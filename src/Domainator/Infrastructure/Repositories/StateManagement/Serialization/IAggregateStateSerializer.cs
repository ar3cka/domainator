using Domainator.Entities;

namespace Domainator.Infrastructure.Repositories.StateManagement.Serialization
{
    /// <summary>
    /// The interface for <see cref="IAggregateState"/> serializer.
    /// </summary>
    public interface IAggregateStateSerializer
    {
        /// <summary>
        /// Serializes the <paramref name="state"/> to string.
        /// </summary>
        /// <param name="state">The state of an aggregate.</param>
        /// <typeparam name="TState">The type of <paramref name="state"/>.</typeparam>
        /// <returns>Returns serialized state as string</returns>
        string SerializeState<TState>(TState state) where TState : class, IAggregateState;

        /// <summary>
        /// Deserializes the state from the <paramref name="serializedState"/>.
        /// </summary>
        /// <param name="serializedState">The serialized as string state of an aggregate.</param>
        /// <typeparam name="TState">The type of state.</typeparam>
        /// <returns>Returns an instance of <typeparamref name="TState"/>.</returns>
        TState DeserializeState<TState>(string serializedState) where TState : class, IAggregateState;

        /// <summary>
        /// Serializes change set object.
        /// </summary>
        /// <param name="changeSet">The change set to serialize..</param>
        /// <returns>Returns serialized change set as string</returns>
        string SerializeChangeSet(ChangeSet changeSet);

        /// <summary>
        /// Deserializes the change set object from <paramref name="serializedChangeSet"/>.
        /// </summary>
        /// <param name="serializedChangeSet">The serialized change set string.</param>
        /// <returns>Returns an instance of <see cref="ChangeSet"/>.</returns>
        ChangeSet DeserializeChangeSet(string serializedChangeSet);
    }
}
