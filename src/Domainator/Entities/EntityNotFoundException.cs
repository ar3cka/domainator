using System;
using System.Runtime.Serialization;

namespace Domainator.Entities
{
    /// <summary>
    /// The error indicates that requested entity is not found.
    /// </summary>
    [Serializable]
    public sealed class EntityNotFoundException : Exception
    {
        public EntityNotFoundException()
        {
        }

        public EntityNotFoundException(string message) : base(message)
        {
        }

        public EntityNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        private EntityNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
