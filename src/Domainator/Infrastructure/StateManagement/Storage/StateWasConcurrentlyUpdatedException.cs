using System;
using System.Runtime.Serialization;

namespace Domainator.Infrastructure.StateManagement.Storage
{
    /// <summary>
    /// The error indicates that during the updated the underlying storage detected concurrent update of the record
    /// with the state data
    /// </summary>
    [Serializable]
    public sealed class StateWasConcurrentlyUpdatedException : Exception
    {
        public StateWasConcurrentlyUpdatedException()
        {
        }

        public StateWasConcurrentlyUpdatedException(string message) : base(message)
        {
        }

        public StateWasConcurrentlyUpdatedException(string message, Exception inner) : base(message, inner)
        {
        }

        private StateWasConcurrentlyUpdatedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
