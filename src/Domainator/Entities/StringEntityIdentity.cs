namespace Domainator.Entities
{
    /// <summary>
    /// A base class for <see cref="string"/> type based identities.
    /// </summary>
    public abstract class StringEntityIdentity : AbstractEntityIdentity<string>
    {
        /// <inheritdoc />
        protected StringEntityIdentity(IEntityIdentity identity) : base(identity)
        {
        }

        /// <inheritdoc />
        protected StringEntityIdentity(string id) : base(id)
        {
        }

        protected override string ParseIdValue(string identityValue) => identityValue;
    }
}
