using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Domainator.Entities;
using Domainator.Infrastructure.Repositories.StateManagement.Serialization;
using Domainator.Utilities;

namespace Domainator.Infrastructure.Repositories.StateManagement.Storage
{
    /// <summary>
    /// Provides the implementation of <see cref="IAggregateStateStorage"/> based on AWS DynamoDB.
    /// </summary>
    public class DynamoDbAggregateStateStorage : IAggregateStateStorage
    {
        private const string HeadSortKeyValue = "HEAD";

        private static readonly List<string> _indexAttributesToGet = new List<string>(2)
        {
            KnownTableAttributes.AggregateId,
            KnownTableAttributes.AggregateType
        };

        private static readonly List<string> _loadAttributesToGet = new List<string>(2)
        {
            KnownTableAttributes.Data,
            KnownTableAttributes.Version
        };

        private static readonly List<string> _loadBatchAttributesToGet = new List<string>(3)
        {
            KnownTableAttributes.PartitionKey,
            KnownTableAttributes.Data,
            KnownTableAttributes.Version
        };

        private readonly Table _dynamoDbTable;
        private readonly IAggregateStateSerializer _serializer;

        public DynamoDbAggregateStateStorage(Table dynamoDbTable, IAggregateStateSerializer serializer)
        {
            Require.NotNull(dynamoDbTable, nameof(dynamoDbTable));
            Require.NotNull(serializer, nameof(serializer));

            _dynamoDbTable = dynamoDbTable;
            _serializer = serializer;
        }

        /// <inheritdoc />
        public async Task<(AggregateVersion, TState)> LoadAsync<TState>(IEntityIdentity id, CancellationToken cancellationToken)
            where TState : class, IAggregateState
        {
            Require.NotNull(id, nameof(id));

            var getConfig = new GetItemOperationConfig
            {
                ConsistentRead = true,
                AttributesToGet = _loadAttributesToGet
            };

            var document = await _dynamoDbTable.GetItemAsync(ConvertToPrimaryKey(id), HeadSortKeyValue, getConfig, cancellationToken);
            if (document != null)
            {
                var restoredData = (string)document[KnownTableAttributes.Data];
                var restoredVersion = (int)document[KnownTableAttributes.Version];
                var state = _serializer.DeserializeState<TState>(restoredData);

                return (AggregateVersion.Create(restoredVersion), state);
            }

            return (AggregateVersion.Emtpy, default);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyDictionary<IEntityIdentity, (AggregateVersion, TState)>> LoadBatchAsync<TState>(
            IReadOnlyCollection<IEntityIdentity> ids, CancellationToken cancellationToken)
            where TState : class, IAggregateState
        {
            Require.NotNull(ids, nameof(ids));

            var batchGet = _dynamoDbTable.CreateBatchGet();
            batchGet.ConsistentRead = true;
            batchGet.AttributesToGet = _loadBatchAttributesToGet;

            var idToValueMap = new Dictionary<string, IEntityIdentity>(ids.Count);
            foreach (var id in ids)
            {
                string primaryKey = ConvertToPrimaryKey(id);

                batchGet.AddKey(primaryKey, HeadSortKeyValue);
                idToValueMap[primaryKey] = id;
            }

            await batchGet.ExecuteAsync(cancellationToken);

            var results = new Dictionary<IEntityIdentity, (AggregateVersion, TState)>(batchGet.Results.Count, EntityIdentity.DefaultComparer);
            foreach (var document in batchGet.Results)
            {
                var partitionKey = (string)document[KnownTableAttributes.PartitionKey];
                var version = (int)document[KnownTableAttributes.Version];
                var data = (string)document[KnownTableAttributes.Data];
                var state = _serializer.DeserializeState<TState>(data);

                results[idToValueMap[partitionKey]] = (AggregateVersion.Create(version), state);
            }

            return results;
        }

        /// <inheritdoc />
        public async Task<FindByAttributeValueStateQueryResult<TState>> FindByAttributeValueAsync<TState>(
            FindByAttributeValueStateQuery query, CancellationToken cancellationToken)
            where TState : class, IAggregateState
        {
            Require.NotNull(query, nameof(query));

            var filter = new QueryFilter();
            filter.AddCondition(query.AttributeName, QueryOperator.Equal, MapValueToDynamoDbEntry(query.AttributeValue));

            var search = _dynamoDbTable.Query(new QueryOperationConfig
            {
                IndexName = $"{query.AttributeName}Index",
                Filter = filter,
                Limit = query.Limit,
                Select = SelectValues.SpecificAttributes,
                AttributesToGet = _indexAttributesToGet,
                PaginationToken = string.IsNullOrEmpty(query.PaginationToken) ? null : query.PaginationToken
            });


            var searchResults = await search.GetNextSetAsync(cancellationToken);
            var identities = new List<IEntityIdentity>(searchResults.Count);
            identities.AddRange(searchResults.Select(item =>
            {
                string aggregateType = item[KnownTableAttributes.AggregateType];
                string aggregateId = item[KnownTableAttributes.AggregateId];
                return new EntityIdentity(aggregateType, aggregateId);
            }));

            var states = await LoadBatchAsync<TState>(identities, cancellationToken);

            return new FindByAttributeValueStateQueryResult<TState>(states, search.IsDone ? null : search.PaginationToken);
        }

        /// <inheritdoc />
        public async Task PersistAsync<TState>(
            IEntityIdentity id, TState state, AggregateVersion version, IReadOnlyDictionary<string, object> attributes, CancellationToken cancellationToken)
            where TState : class, IAggregateState
        {
            Require.NotNull(id, nameof(id));
            Require.NotNull(state, nameof(state));
            Require.NotNull(attributes, nameof(attributes));

            var document = new Document();

            FillKnownAttributes(id, state, version, document);
            FillCustomAttributes(attributes, document);

            try
            {
                if (version == AggregateVersion.Emtpy)
                {
                    await PutItemAsync(document, cancellationToken);
                }
                else
                {
                    await UpdateItemAsync(version, document, cancellationToken);
                }
            }
            catch (ConditionalCheckFailedException exception)
            {
                throw new StateWasConcurrentlyUpdatedException(
                    $"The version \"{version.ToString()}\" of the aggregate state \"{id}\" is not the latest",
                    exception);
            }
        }

        private async Task UpdateItemAsync(AggregateVersion version,Document document, CancellationToken cancellationToken)
        {
            var updateConfig = new UpdateItemOperationConfig();
            updateConfig.ExpectedState = new ExpectedState();
            updateConfig.ExpectedState.AddExpected(KnownTableAttributes.Version, ScanOperator.Equal, (int)version);

            await _dynamoDbTable.UpdateItemAsync(document, updateConfig, cancellationToken);
        }

        private async Task PutItemAsync(Document document, CancellationToken cancellationToken)
        {
            var putConfig = new PutItemOperationConfig();
            putConfig.ExpectedState = new ExpectedState();
            putConfig.ExpectedState.AddExpected(KnownTableAttributes.PartitionKey, exists: false);
            putConfig.ExpectedState.AddExpected(KnownTableAttributes.SortKey, exists: false);

            await _dynamoDbTable.PutItemAsync(document, putConfig, cancellationToken);
        }

        private void FillKnownAttributes<TState>(
            IEntityIdentity id, TState state, AggregateVersion version, IDictionary<string, DynamoDBEntry> document)
            where TState : class, IAggregateState
        {
            var changeSet = new ChangeSet(version, state.GetChanges());

            document[KnownTableAttributes.AggregateId] = id.Value;
            document[KnownTableAttributes.AggregateType] = id.Tag;
            document[KnownTableAttributes.PartitionKey] = ConvertToPrimaryKey(id);
            document[KnownTableAttributes.SortKey] = HeadSortKeyValue;
            document[KnownTableAttributes.Data] = _serializer.SerializeState(state);
            document[KnownTableAttributes.Version] = (int)changeSet.ToVersion;

            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (version == AggregateVersion.Emtpy)
            {
                document[KnownTableAttributes.CreatedAt] = now;
            }

            document[KnownTableAttributes.UpdatedAt] = now;
            document[KnownTableAttributes.LastChangeSet] = _serializer.SerializeChangeSet(changeSet);
        }

        private static void FillCustomAttributes(IReadOnlyDictionary<string, object> attributes, IDictionary<string, DynamoDBEntry> document)
        {
            foreach (var attribute in attributes)
            {
                var dbEntry = MapValueToDynamoDbEntry(attribute.Value);
                if (dbEntry != null)
                {
                    document[attribute.Key] = dbEntry;
                }
            }
        }

        private static DynamoDBEntry MapValueToDynamoDbEntry(object value) => Type.GetTypeCode(value.GetType()) switch
        {
            TypeCode.Boolean => (bool)value,
            TypeCode.Byte => (byte)value,
            TypeCode.Char => (char)value,
            TypeCode.DateTime => (DateTime)value,
            TypeCode.Decimal => (decimal)value,
            TypeCode.Double => (double)value,
            TypeCode.Empty => null,
            TypeCode.Int16 => (short)value,
            TypeCode.Int32 => (int)value,
            TypeCode.Int64 => (long)value,
            TypeCode.SByte => (sbyte)value,
            TypeCode.Single => (float)value,
            TypeCode.String => (string)value,
            TypeCode.UInt16 => (ushort)value,
            TypeCode.UInt32 => (uint)value,
            TypeCode.UInt64 => (ulong)value,
            TypeCode.Object => MapObjectToDynamoDbEntry(value),
            _ => throw NotSupportedAttributeType(value.GetType())
        };

        private static DynamoDBEntry MapObjectToDynamoDbEntry(object value) =>
            AbstractEntityIdentity.IsValidType(value.GetType())
                ? MapValueToDynamoDbEntry(AbstractEntityIdentity.ExtractRawValue(value))
                : throw NotSupportedAttributeType(value.GetType());

        private static string ConvertToPrimaryKey(IEntityIdentity id) => id.Tag + "_" + id.Value;

        private static InvalidOperationException NotSupportedAttributeType(Type valueType) =>
            new InvalidOperationException($"Attributes of type {valueType} are not supported.");
    }
}
