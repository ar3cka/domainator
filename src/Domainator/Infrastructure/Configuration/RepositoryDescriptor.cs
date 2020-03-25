using System;
using Domainator.Utilities;

namespace Domainator.Infrastructure.Configuration
{
    /// <summary>
    /// Provides the description of a repository type.
    /// </summary>
    public sealed class RepositoryDescriptor
    {
        /// <summary>
        /// Gets the type of repository's interface
        /// </summary>
        public Type InterfaceType { get; }

        /// <summary>
        /// Gets the implementation type of repository's interface
        /// </summary>
        public Type ImplementationType { get; }

        public RepositoryDescriptor(Type interfaceType, Type implementationType)
        {
            Require.NotNull(interfaceType, nameof(interfaceType));
            Require.NotNull(implementationType, nameof(implementationType));

            InterfaceType = interfaceType;
            ImplementationType = implementationType;
        }
    }
}
