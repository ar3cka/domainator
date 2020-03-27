using System;
using System.Collections.Generic;
using System.Globalization;
using Domainator.Utilities;

namespace Domainator.Entities
{
    /// <summary>
    /// A base class for the scalar type based identities such as <see cref="Int32"/>, <see cref="string"/>,
    /// <see cref="Guid"/>, etc. The class implements basic equality functionality.
    /// </summary>
    /// <typeparam name="TKey">The type of scalar type.</typeparam>
    public abstract class AbstractEntityIdentity<TKey> : IEntityIdentity, IEquatable<AbstractEntityIdentity<TKey>>
    {
        /// <summary>
        /// The constructor is used for restoring the instance of <see cref="AbstractEntityIdentity{TKey}"/>
        /// from provided <see cref="IEntityIdentity"/>. It is used during restoring aggregates from persistence
        /// storage.
        /// </summary>
        protected AbstractEntityIdentity(IEntityIdentity identity)
        {
            Require.True(
                identity.Tag.Equals(Tag, StringComparison.Ordinal), nameof(identity), "Invalid identity.Tag value");

            Id = ParseIdValue(identity.Value);
        }

        /// <summary>
        /// The constructor is used for creating new value of <see cref="AbstractEntityIdentity{TKey}"/>.
        /// </summary>
        /// <param name="id"></param>
        protected AbstractEntityIdentity(TKey id)
        {
            Id = id;
        }

        /// <summary>
        /// Parse received value from <see cref="IEntityIdentity.Value"/> back to <see cref="Id"/>.
        /// </summary>
        /// <param name="identityValue"></param>
        /// <returns></returns>
        protected abstract TKey ParseIdValue(string identityValue);

        /// <summary>
        /// Gets internal value of the identity as CLR type.
        /// </summary>
        public TKey Id { get; }

        /// <inheritdoc />
        public abstract string Tag { get; }

        /// <inheritdoc />
        public virtual string Value => Id.ToString();

        /// <inheritdoc />
        public virtual bool Equals(AbstractEntityIdentity<TKey> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return EqualityComparer<TKey>.Default.Equals(Id, other.Id) && Tag.Equals(other.Tag, StringComparison.Ordinal);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((AbstractEntityIdentity<TKey>) obj);
        }

        /// <summary>
        /// Returns a hash code value calculated based on <see cref="Id"/> and <see cref="Tag"/> properties.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<TKey>.Default.GetHashCode(Id) * 397) ^ Tag.GetHashCode();
            }
        }

        public static bool operator ==(AbstractEntityIdentity<TKey> left, AbstractEntityIdentity<TKey> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AbstractEntityIdentity<TKey> left, AbstractEntityIdentity<TKey> right)
        {
            return !Equals(left, right);
        }

        public override string ToString() => string.Format(CultureInfo.InvariantCulture, "{0}|{1}", Tag, Value);
    }
}
