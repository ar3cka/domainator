using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Domainator.Entities;
using Domainator.Utilities;
using Newtonsoft.Json;

namespace Domainator.Infrastructure.StateManagement
{
    /// <summary>
    /// Provides the implementation of <see cref="IAggregateStateStorage{TState}"/> based on AWS DynamoDB.
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    public class DynamoDbAggregateStateStorage<TState> : IAggregateStateStorage<TState> where TState : class, IAggregateState
    {
        private const string HeadSortKeyValue = "HEAD";

        private readonly Table _dynamoDbTable;


        public DynamoDbAggregateStateStorage(Table dynamoDbTable)
        {
            Require.NotNull(dynamoDbTable, nameof(dynamoDbTable));

            _dynamoDbTable = dynamoDbTable;
        }

        /// <inheritdoc />
        public async Task<(AggregateVersion, TState)> LoadAsync(IEntityIdentity id, CancellationToken cancellationToken)
        {
            Require.NotNull(id, nameof(id));

            var getConfig = new GetItemOperationConfig {ConsistentRead = true};
            var document = await _dynamoDbTable.GetItemAsync(ConvertToPrimaryKey(id), HeadSortKeyValue, getConfig, cancellationToken);
            if (document != null)
            {
                var restoredData = (string)document[KnownTableAttributes.Data];
                var restoredVersion = (int)document[KnownTableAttributes.Version];
                var state = JsonConvert.DeserializeObject<TState>(restoredData);

                return (AggregateVersion.Create(restoredVersion), state);
            }

            return (AggregateVersion.Emtpy, default);
        }

        /// <inheritdoc />
        public async Task PersistAsync(IEntityIdentity id, TState state, AggregateVersion version, CancellationToken cancellationToken)
        {
            Require.NotNull(id, nameof(id));
            Require.NotNull(state, nameof(state));

            var stateVersion = version.Increment(state.Changes.Count);

            var document = new Document();
            document[KnownTableAttributes.AggregateType] = id.Tag;
            document[KnownTableAttributes.PartitionKey] = ConvertToPrimaryKey(id);
            document[KnownTableAttributes.SortKey] = HeadSortKeyValue;
            document[KnownTableAttributes.Data] = JsonConvert.SerializeObject(state);
            document[KnownTableAttributes.Version] = (int)stateVersion;

            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (version == AggregateVersion.Emtpy)
            {
                document[KnownTableAttributes.CreatedAt] = now;
            }

            document[KnownTableAttributes.UpdatedAt] = now;

            var putConfig = new PutItemOperationConfig();
            putConfig.ExpectedState = new ExpectedState();
            putConfig.ExpectedState.ConditionalOperator = ConditionalOperatorValues.Or;
            putConfig.ExpectedState.AddExpected(KnownTableAttributes.Version, ScanOperator.Equal, (int)version);
            putConfig.ExpectedState.AddExpected(KnownTableAttributes.Version, exists: false);

            try
            {
                await _dynamoDbTable.PutItemAsync(document, putConfig, cancellationToken);
            }
            catch (ConditionalCheckFailedException exception)
            {
                throw new StateWasConcurrentlyUpdatedException(
                    $"The version \"{version}\" of the aggregate state \"{id}\" is not the latest",
                    exception);
            }
        }

        private static string ConvertToPrimaryKey(IEntityIdentity id) => id.Tag + "|" + id.Value;
    }
}