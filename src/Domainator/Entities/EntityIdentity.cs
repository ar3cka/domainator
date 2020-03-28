using System.Collections.Generic;
using Domainator.Utilities;

namespace Domainator.Entities
{
    /// <summary>
    /// Untyped unique identity of the entity
    /// </summary>
    public sealed partial class EntityIdentity : IEntityIdentity
    {
        /// <summary>
        /// Default equality comparer that can be used for comparison of strongly typed
        /// identities (<see cref="AbstractEntityIdentity{TKey}"/>) and untyped identities <see cref="EntityIdentity"/>.
        /// </summary>
        public static readonly IEqualityComparer<IEntityIdentity> DefaultComparer = new EqualityComparer();

        /// <inheritdoc />
        public string Tag { get; }

        /// <inheritdoc />
        public string Value { get; }

        public EntityIdentity(string tag, string value)
        {
            Require.NotEmpty(tag, nameof(tag));
            Require.NotEmpty(value, nameof(value));

            Tag = tag;
            Value = value;
        }
    }
}
