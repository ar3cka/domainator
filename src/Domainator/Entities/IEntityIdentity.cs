namespace Domainator.Entities
{
    /// <summary>
    /// The interface provides a set of properties that all unique identities of entities are expected to expose.
    /// Infrastructural components can use the interface when the identity needs to be saved or restored from
    /// external storage.
    /// </summary>
    public interface IEntityIdentity
    {
        /// <summary>
        /// Gets the unique tag (type) of the entity within domain.
        /// </summary>
        string Tag { get; }

        /// <summary>
        /// Gets the unique value of the identity.
        /// </summary>
        string Value { get; }
    }
}
