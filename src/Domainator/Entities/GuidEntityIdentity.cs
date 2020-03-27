using System;

namespace Domainator.Entities
{
    /// <summary>
    /// A base class for <see cref="Guid"/> type based identities.
    /// </summary>
    public abstract class GuidEntityIdentity : AbstractEntityIdentity<Guid>
    {
        /// <inheritdoc />
        public override string Value => Id.ToString("D");

        /// <inheritdoc />
        protected GuidEntityIdentity(IEntityIdentity identity) : base(identity)
        {
        }

        /// <inheritdoc />
        protected GuidEntityIdentity(Guid id) : base(id)
        {
        }

        protected override Guid ParseIdValue(string identityValue) => Guid.Parse(identityValue);
    }
}
