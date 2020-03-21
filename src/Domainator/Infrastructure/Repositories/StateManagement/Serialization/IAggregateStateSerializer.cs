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
        string Serialize<TState>(TState state) where TState : class, IAggregateState;

        /// <summary>
        /// Deserializes the state from the <paramref name="serializedState"/>.
        /// </summary>
        /// <param name="serializedState">The serialized as string state of an aggregate.</param>
        /// <typeparam name="TState">The type of state.</typeparam>
        /// <returns>Returns an instance of <typeparamref name="TState"/>.</returns>
        TState Deserialize<TState>(string serializedState) where TState : class, IAggregateState;
    }
}
