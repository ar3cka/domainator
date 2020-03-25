using System;
using System.Collections.Generic;
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

            var getConfig = new GetItemOperationConfig {ConsistentRead = true};
            var document = await _dynamoDbTable.GetItemAsync(ConvertToPrimaryKey(id), HeadSortKeyValue, getConfig, cancellationToken);
            if (document != null)
            {
                var restoredData = (string)document[KnownTableAttributes.Data];
                var restoredVersion = (int)document[KnownTableAttributes.Version];
                var state = _serializer.Deserialize<TState>(restoredData);

                return (AggregateVersion.Create(restoredVersion), state);
            }

            return (AggregateVersion.Emtpy, default);
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
                    $"The version \"{version.ToString()}\" of the aggregate state \"{id}\" is not the latest",
                    exception);
            }
        }

        private void FillKnownAttributes<TState>(IEntityIdentity id, TState state, AggregateVersion version,
            Document document) where TState : class, IAggregateState
        {
            var stateVersion = version.Increment(state.GetChanges().Count);
            document[KnownTableAttributes.AggregateType] = id.Tag;
            document[KnownTableAttributes.PartitionKey] = ConvertToPrimaryKey(id);
            document[KnownTableAttributes.SortKey] = HeadSortKeyValue;
            document[KnownTableAttributes.Data] = _serializer.Serialize(state);
            document[KnownTableAttributes.Version] = (int)stateVersion;

            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (version == AggregateVersion.Emtpy)
            {
                document[KnownTableAttributes.CreatedAt] = now;
            }

            document[KnownTableAttributes.UpdatedAt] = now;
        }

        private static void FillCustomAttributes(IReadOnlyDictionary<string, object> attributes, Document document)
        {
            foreach (var attribute in attributes)
            {
                switch (Type.GetTypeCode(attribute.Value.GetType()))
                {
                    case TypeCode.Boolean:
                        document[attribute.Key] = (bool)attribute.Value;
                        break;
                    case TypeCode.Byte:
                        document[attribute.Key] = (byte)attribute.Value;
                        break;
                    case TypeCode.Char:
                        document[attribute.Key] = (char)attribute.Value;
                        break;
                    case TypeCode.DateTime:
                        document[attribute.Key] = (DateTime)attribute.Value;
                        break;
                    case TypeCode.Decimal:
                        document[attribute.Key] = (decimal)attribute.Value;
                        break;
                    case TypeCode.Double:
                        document[attribute.Key] = (double)attribute.Value;
                        break;
                    case TypeCode.Int16:
                        document[attribute.Key] = (short)attribute.Value;
                        break;
                    case TypeCode.Int32:
                        document[attribute.Key] = (int)attribute.Value;
                        break;
                    case TypeCode.Int64:
                        document[attribute.Key] = (long)attribute.Value;
                        break;
                    case TypeCode.SByte:
                        document[attribute.Key] = (sbyte)attribute.Value;
                        break;
                    case TypeCode.Single:
                        document[attribute.Key] = (float)attribute.Value;
                        break;
                    case TypeCode.String:
                        document[attribute.Key] = (string)attribute.Value;
                        break;
                    case TypeCode.UInt16:
                        document[attribute.Key] = (ushort)attribute.Value;
                        break;
                    case TypeCode.UInt32:
                        document[attribute.Key] = (uint)attribute.Value;
                        break;
                    case TypeCode.UInt64:
                        document[attribute.Key] = (ulong)attribute.Value;
                        break;
                    case TypeCode.DBNull:
                    case TypeCode.Empty:
                        // skip null values
                        break;
                    default:
                        throw new InvalidOperationException($"Custom attributes of type {attribute.Value.GetType()} are not supported.");
                }
            }
        }

        private static string ConvertToPrimaryKey(IEntityIdentity id) => id.Tag + "|" + id.Value;
    }
}
