using Domainator.Utilities;

namespace Domainator.Infrastructure.Repositories.StateManagement.Storage
{
    /// <summary>
    /// Query object for retrieving aggregate states by custom attribute value.
    /// </summary>
    public sealed class FindByAttributeValueStateQuery
    {
        /// <summary>
        /// Gets the name of the attribute for query.
        /// </summary>
        public string AttributeName { get; }

        /// <summary>
        /// Gets the value of the attribute for query.
        /// </summary>
        public object AttributeValue { get; }

        /// <summary>
        /// Gets the pagination token for retrieving next page of results
        /// </summary>
        public string PaginationToken { get; }

        /// <summary>
        /// Gets the limit for the number of items to retrieve.
        /// </summary>
        public int Limit { get; }

        public FindByAttributeValueStateQuery(string attributeName, object attributeValue, int limit)
        {
            Require.NotEmpty(attributeName, nameof(attributeName));
            Require.NotNull(attributeValue, nameof(attributeValue));
            Require.Positive(limit, nameof(limit));

            AttributeName = attributeName;
            AttributeValue = attributeValue;
            Limit = limit;
        }

        public FindByAttributeValueStateQuery(string attributeName, object attributeValue, int limit, string paginationToken)
            : this(attributeName, attributeValue, limit)
        {
            PaginationToken = paginationToken;
        }
    }
}
