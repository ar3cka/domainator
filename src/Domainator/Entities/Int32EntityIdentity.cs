using System.Globalization;

namespace Domainator.Entities
{
    /// <summary>
    /// A base class for <see cref="int"/> type based identities.
    /// </summary>
    public abstract class Int32EntityIdentity : AbstractEntityIdentity<int>
    {
        /// <inheritdoc />
        protected Int32EntityIdentity(IEntityIdentity identity) : base(identity)
        {
        }

        /// <inheritdoc />
        protected Int32EntityIdentity(int id) : base(id)
        {
        }

        protected override int ParseIdValue(string identityValue) =>
            int.Parse(identityValue, NumberStyles.Integer, CultureInfo.InvariantCulture);
    }
}
