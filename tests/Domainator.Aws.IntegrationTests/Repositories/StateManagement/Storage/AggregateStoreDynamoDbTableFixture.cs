using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Domainator.Infrastructure.Repositories.StateManagement.Storage;

namespace Domainator.Aws.IntegrationTests.Repositories.StateManagement.Storage
{
    public class AggregateStoreDynamoDbTableFixture
    {
        public Table StoreTable { get; }

        public AggregateStoreDynamoDbTableFixture()
        {
            StoreTable = PrepareTable();
        }

        private static Table PrepareTable()
        {
            var client = new AmazonDynamoDBClient(new AmazonDynamoDBConfig
            {
                ServiceURL = "http://localhost:8000"
            });

            var tables = client.ListTablesAsync().GetAwaiter().GetResult();
            if (!tables.TableNames.Contains("AggregateStore"))
            {
                var response = client.CreateTableAsync(new CreateTableRequest(
                        tableName: "AggregateStore",
                        keySchema: new List<KeySchemaElement>
                        {
                            new KeySchemaElement(KnownTableAttributes.PartitionKey, KeyType.HASH),
                            new KeySchemaElement(KnownTableAttributes.SortKey, KeyType.RANGE)
                        },
                        attributeDefinitions: new List<AttributeDefinition>
                        {
                            new AttributeDefinition(KnownTableAttributes.PartitionKey, ScalarAttributeType.S),
                            new AttributeDefinition(KnownTableAttributes.SortKey, ScalarAttributeType.S),
                        },
                        provisionedThroughput: new ProvisionedThroughput(1, 1)))
                    .GetAwaiter()
                    .GetResult();

                var createIndex = new UpdateTableRequest();
                createIndex.TableName = "AggregateStore";

                createIndex.AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition("TaskProjectId", ScalarAttributeType.N)
                };

                createIndex.GlobalSecondaryIndexUpdates = new List<GlobalSecondaryIndexUpdate>();
                createIndex.GlobalSecondaryIndexUpdates.Add(new GlobalSecondaryIndexUpdate
                {
                    Create = new CreateGlobalSecondaryIndexAction
                    {
                        IndexName = "TaskProjectIdIndex",
                        Projection = new Projection
                        {
                            ProjectionType = ProjectionType.INCLUDE,
                            NonKeyAttributes = new List<string>
                            {
                                KnownTableAttributes.AggregateId,
                                KnownTableAttributes.AggregateType
                            }
                        },
                        KeySchema = new List<KeySchemaElement>
                        {
                            new KeySchemaElement("TaskProjectId", KeyType.HASH)
                        },
                        ProvisionedThroughput = new ProvisionedThroughput(1, 1)
                    }
                });

                client.UpdateTableAsync(createIndex).GetAwaiter().GetResult();
            }

            return Table.LoadTable(client, "AggregateStore");
        }
    }
}
