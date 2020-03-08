using System;
using System.Collections.Generic;

namespace Domainator.Entities
{
    public abstract class AbstractEntityIdentity<TKey> : IEntityIdentity, IEquatable<AbstractEntityIdentity<TKey>> where TKey : notnull
    {
        public abstract TKey Id { get; protected set; }
        
        public abstract string Tag { get; }

        public virtual string Value => Id.ToString();
        
        public bool Equals(AbstractEntityIdentity<TKey>? other)
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

        public override bool Equals(object? obj)
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

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Tag);
        }

        public static bool operator ==(AbstractEntityIdentity<TKey>? left, AbstractEntityIdentity<TKey>? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AbstractEntityIdentity<TKey>? left, AbstractEntityIdentity<TKey>? right)
        {
            return !Equals(left, right);
        }
    }
}