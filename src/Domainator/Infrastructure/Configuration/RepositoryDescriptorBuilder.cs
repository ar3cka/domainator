using Domainator.Entities;

namespace Domainator.Infrastructure.Configuration
{
    /// <summary>
    /// Configuration description builder for a repository.
    /// </summary>
    public class RepositoryDescriptorBuilder<TEntityId, TAggregateRoot>
        where TEntityId : class, IEntityIdentity
        where TAggregateRoot : class, IAggregateRoot
    {
        /// <summary>
        /// Registers the repository type for <typeparamref name="TAggregateRoot"/>.
        /// </summary>
        public RepositoryDescriptorBuilder<TEntityId, TAggregateRoot> UseRepository<TRepositoryInterface, TImplementation>()
            where TImplementation : TRepositoryInterface
            where TRepositoryInterface : IRepository<TEntityId, TAggregateRoot>
        {
            var implementationType = typeof(TImplementation);
            var interfaceType = typeof(TRepositoryInterface);

            RepositoryDescriptor = new RepositoryDescriptor(interfaceType, implementationType);

            return this;
        }

        /// <summary>
        /// Gets the configured repository descriptor.
        /// </summary>
        public RepositoryDescriptor RepositoryDescriptor { get; private set; }
    }
}
