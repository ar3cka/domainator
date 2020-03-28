using System;
using System.Collections.Generic;

namespace Domainator.Entities
{
    public partial class EntityIdentity
    {
        private sealed class EqualityComparer : IEqualityComparer<IEntityIdentity>
        {
            public bool Equals(IEntityIdentity x, IEntityIdentity y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (ReferenceEquals(x, null))
                {
                    return false;
                }

                if (ReferenceEquals(y, null))
                {
                    return false;
                }

                return x.Tag.Equals(y.Tag, StringComparison.Ordinal) && x.Value.Equals(y.Value, StringComparison.Ordinal);
            }

            public int GetHashCode(IEntityIdentity obj)
            {
                unchecked
                {
                    return (obj.Tag.GetHashCode() * 397) ^ obj.Value.GetHashCode();
                }
            }
        }
    }
}
