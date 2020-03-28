using System.Collections.Generic;
using Domainator.Utilities;

namespace Domainator.Infrastructure.Repositories
{
    /// <summary>
    /// The result of a query to repository.
    /// </summary>
    /// <typeparam name="TItem">The type of the queried item.</typeparam>
    public class RepositoryQueryResult<TItem>
    {
        /// <summary>
        /// Gets the list of retrieved items
        /// </summary>
        public IReadOnlyList<TItem> Items { get; }

        /// <summary>
        /// Gets the pagination token value. If there are no more items, returns null.
        /// </summary>
        public string PaginationToken { get; }

        public RepositoryQueryResult(IReadOnlyList<TItem> items, string paginationToken)
        {
            Require.NotNull(items, nameof(items));

            Items = items;
            PaginationToken = paginationToken;
        }
    }
}
