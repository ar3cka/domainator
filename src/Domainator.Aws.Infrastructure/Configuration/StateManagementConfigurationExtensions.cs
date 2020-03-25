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
                var serilaizer = (IAggregateStateSerializer)serviceProvider(typeof(IAggregateStateSerializer));
                var dynamoDb = (IAmazonDynamoDB)serviceProvider(typeof(IAmazonDynamoDB));
                var table = Table.LoadTable(dynamoDb, aggregateStoreTableName);

                return new DynamoDbAggregateStateStorage(table, serilaizer);
            });
        }
    }
}
