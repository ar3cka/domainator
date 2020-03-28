using System.Collections.Generic;
using Domainator.Entities;
using Domainator.Utilities;

namespace Domainator.Infrastructure.Repositories.StateManagement.Storage
{
    /// <summary>
    /// A result of <see cref="FindByAttributeValueStateQuery"/> query.
    /// </summary>
    /// <typeparam name="TState">The type of the state</typeparam>
    public sealed class FindByAttributeValueStateQueryResult<TState>
        where TState : IAggregateState
    {
        /// <summary>
        /// Gets the map of retrieved identities and states.
        /// </summary>
        public IReadOnlyDictionary<IEntityIdentity, (AggregateVersion, TState)> States { get; }

        /// <summary>
        /// Gets the pagination token value. If there are no more items, returns null.
        /// </summary>
        public string PaginationToken { get; }

        public FindByAttributeValueStateQueryResult(
            IReadOnlyDictionary<IEntityIdentity,(AggregateVersion, TState)> states,
            string paginationToken)
        {
            Require.NotNull(states, nameof(states));

            States = states;
            PaginationToken = paginationToken;
        }
    }
}
