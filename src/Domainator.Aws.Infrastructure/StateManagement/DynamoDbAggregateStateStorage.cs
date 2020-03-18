using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Domainator.Entities;
using Domainator.Utilities;
using Newtonsoft.Json;

namespace Domainator.Infrastructure.StateManagement
{
    /// <summary>
    /// The error indicates that during the updated the underlying storage detected concurrent update of the record
    /// with the state data
    /// </summary>
    [Serializable]
    public sealed class StateWasConcurrentlyUpdatedException : Exception
    {
        public StateWasConcurrentlyUpdatedException()
        {
        }

        public StateWasConcurrentlyUpdatedException(string message) : base(message)
        {
        }

        public StateWasConcurrentlyUpdatedException(string message, Exception inner) : base(message, inner)
        {
        }

        private StateWasConcurrentlyUpdatedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class DynamoDbAggregateStateStorage<TState> : IAggregateStateStorage<TState> where TState : class, IAggregateState
    {
        private static class TableAttributes
        {
            public const string PartitionKey = "PK";
            public const string SortKey = "SK";
            public const string Data = "Data";
            public const string Version = "Version";
        }

        private const string HeadSortKeyValue = "HEAD";

        public async Task<(AggregateVersion, TState)> LoadAsync(IEntityIdentity id, CancellationToken cancellationToken)
        {
            Require.NotNull(id, nameof(id));

            var table = await PrepareTableAsync(cancellationToken);

            var getConfig = new GetItemOperationConfig {ConsistentRead = true};

            var document = await table.GetItemAsync(ConvertToPrimaryKey(id), HeadSortKeyValue, getConfig, cancellationToken);
            if (document != null)
            {
                var restoredData = (string)document[TableAttributes.Data];
                var restoredVersion = (int)document[TableAttributes.Version];
                var state = JsonConvert.DeserializeObject<TState>(restoredData);

                return (AggregateVersion.Create(restoredVersion), state);
            }

            return (AggregateVersion.Emtpy, default);
        }

        public async Task PersistAsync(IEntityIdentity id, TState state, AggregateVersion version, CancellationToken cancellationToken)
        {
            Require.NotNull(id, nameof(id));
            Require.NotNull(state, nameof(state));

            var table = await PrepareTableAsync(cancellationToken);
            var stateVersion = version.Increment(state.Changes.Count);

            var document = new Document();
            document[TableAttributes.PartitionKey] = ConvertToPrimaryKey(id);
            document[TableAttributes.SortKey] = HeadSortKeyValue;
            document[TableAttributes.Data] = JsonConvert.SerializeObject(state);
            document[TableAttributes.Version] = (int)stateVersion;

            var putConfig = new PutItemOperationConfig();
            putConfig.ExpectedState = new ExpectedState();
            putConfig.ExpectedState.ConditionalOperator = ConditionalOperatorValues.Or;
            putConfig.ExpectedState.AddExpected(TableAttributes.Version, ScanOperator.Equal, (int)version);
            putConfig.ExpectedState.AddExpected(TableAttributes.Version, exists: false);

            try
            {
                await table.PutItemAsync(document, putConfig, cancellationToken);
            }
            catch (ConditionalCheckFailedException exception)
            {
                throw new StateWasConcurrentlyUpdatedException(
                    $"The version \"{version}\" of the aggregate state \"{id}\" is not the latest",
                    exception);
            }
        }

        private static string ConvertToPrimaryKey(IEntityIdentity id) => id.Tag + "|" + id.Value;

        private static async Task<Table> PrepareTableAsync(CancellationToken cancellationToken)
        {
            var client = new AmazonDynamoDBClient(new AmazonDynamoDBConfig
            {
                ServiceURL = "http://localhost:8000"
            });

            var tables = await client.ListTablesAsync(cancellationToken);
            if (!tables.TableNames.Contains("AggregateStore"))
            {
                var response = await client.CreateTableAsync(new CreateTableRequest(
                        tableName: "AggregateStore",
                        keySchema: new List<KeySchemaElement>
                        {
                            new KeySchemaElement(TableAttributes.PartitionKey, KeyType.HASH),
                            new KeySchemaElement(TableAttributes.SortKey, KeyType.RANGE)
                        },
                        attributeDefinitions: new List<AttributeDefinition>
                        {
                            new AttributeDefinition(TableAttributes.PartitionKey, ScalarAttributeType.S),
                            new AttributeDefinition(TableAttributes.SortKey, ScalarAttributeType.S)
                        },
                        provisionedThroughput: new ProvisionedThroughput(1, 1)),
                    cancellationToken: cancellationToken);
            }

            return Table.LoadTable(client, "AggregateStore");
        }
    }
}
