using System;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Domainator.Infrastructure.Repositories.StateManagement.Serialization;
using Domainator.Infrastructure.Repositories.StateManagement.Storage;
using Domainator.Utilities;

namespace Domainator.Infrastructure.Configuration
{
    public static class DynamoDbStateManagementConfigurationExtensions
    {
        public static IDomainatorInfrastructureBuilder UseDynamoDb(this StateManagementConfiguration configuration)
        {
            Require.NotNull(configuration, nameof(configuration));

            return configuration.UseDynamoDb("AggregateStore");
        }

        public static IDomainatorInfrastructureBuilder UseDynamoDb(
            this StateManagementConfiguration configuration, string aggregateStoreTableName)
        {
            Require.NotNull(configuration, nameof(configuration));
            Require.NotEmpty(aggregateStoreTableName, nameof(aggregateStoreTableName));

            return configuration.UseStateStorageFactory(serviceProvider =>
            {
                var serializer = (IAggregateStateSerializer)serviceProvider(typeof(IAggregateStateSerializer));
                var dynamoDb = (IAmazonDynamoDB)serviceProvider(typeof(IAmazonDynamoDB));
                // AWSSDK v4 have deprecated all of Table.LoadTable methods that relied on table introspection via DescribeTable call
                // unfortunately because Domainator has to support GSI defined on client-side ad-hoc (see $"{query.AttributeName}Index")
                // it's not currently possible to avoid introspection.
                // As of v4.0.10.1 Table.LoadTable method, although marked as Obsolete, is still functioning.
                // In future versions of AWSSDK, in case if method would be removed, we'd have to either replicate introspection logic
                // or pass the burden of configuring ITable on to consumer.
#pragma warning disable CS0618 // Type or member is obsolete
                var table = Table.LoadTable(dynamoDb, aggregateStoreTableName);
#pragma warning restore CS0618 // Type or member is obsolete

                return new DynamoDbAggregateStateStorage(table, serializer);
            });
        }

        [CLSCompliant(false)]
        public static IDomainatorInfrastructureBuilder UseDynamoDb(
            this StateManagementConfiguration configuration, ITable aggregateStoreTable)
        {
            Require.NotNull(configuration, nameof(configuration));
            Require.NotNull(aggregateStoreTable, nameof(aggregateStoreTable));

            return configuration.UseStateStorageFactory(serviceProvider =>
            {
                var serializer = (IAggregateStateSerializer)serviceProvider(typeof(IAggregateStateSerializer));

                return new DynamoDbAggregateStateStorage(aggregateStoreTable, serializer);
            });
        }
    }
}
