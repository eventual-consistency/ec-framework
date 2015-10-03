using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventualConsistency.Framework;
using EventualConsistency.Framework.Infrastructure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace EventualConsistency.Providers.Azure
{
    /// <summary>
    ///     Event storage in Azure storage containers/tables.
    /// </summary>
    /// <typeparam name="TAggregateType">Type of aggregate</typeparam>
    /// <typeparam name="TKeyType">Key type of aggregate</typeparam>
    public class AzureTableStorageRepository<TAggregateType, TKeyType> : IRepository<TAggregateType, TKeyType>
        where TAggregateType : class, IKeyedAggregate<TKeyType>
        where TKeyType : IAggregateIdentity
    {
        /// <summary>
        ///     Initialize a new instance of the repository.
        /// </summary>
        /// <param name="aggregateFactory">Aggregate factory method</param>
        /// <param name="context">Repository operating context</param>
        /// <param name="snapshotPolicy">Snapshot Policy</param>
        /// <param name="snapshotProvider">Snapshot Provider</param>
        /// <param name="eventWriter">Event Bus Writer</param>
        /// <param name="table">Storage table for events</param>
        /// <param name="resolver">Type Resolver</param>
        public AzureTableStorageRepository(Func<TAggregateType> aggregateFactory, CloudTable table,
            ISnapshotProvider<TAggregateType, TKeyType> snapshotProvider,
            ISnapshotPolicy<TAggregateType, TKeyType> snapshotPolicy, IEventBusWriter eventWriter,
            IBoundedContext context,
            ITypeResolver resolver)
        {
            AggregateFactory = aggregateFactory;
            EventStoreTable = table;
            Context = context;
            SnapshotPolicy = snapshotPolicy;
            SnapshotProvider = snapshotProvider;
            EventBus = eventWriter;
            Resolver = resolver;
        }

        /// <summary>
        ///     Aggregate Factory
        /// </summary>
        protected Func<TAggregateType> AggregateFactory { get; }

        /// <summary>
        ///     Event Store Table
        /// </summary>
        protected CloudTable EventStoreTable { get; }

        /// <summary>
        ///     Type Resolver
        /// </summary>
        protected ITypeResolver Resolver { get; }

        /// <summary>
        ///     Snapsot Policy
        /// </summary>
        protected ISnapshotPolicy<TAggregateType, TKeyType> SnapshotPolicy { get; }

        /// <summary>
        ///     Snapshot Provider
        /// </summary>
        protected ISnapshotProvider<TAggregateType, TKeyType> SnapshotProvider { get; }

        /// <summary>
        ///     Known event types (for mapping back stored data to objects)
        /// </summary>
        protected IBoundedContext Context { get; }

        /// <summary>
        ///     Event bus to write to post-commit
        /// </summary>
        protected IEventBusWriter EventBus { get; }

        /// <summary>
        ///     Load an aggregate by it's key
        /// </summary>
        /// <param name="key">Key value for aggregate</param>
        /// <returns></returns>
        public TAggregateType Get(TKeyType key)
        {
            // Stage 1: Most recent snapshot or blank
            var aggregate = GetBlank(key);
            if (SnapshotProvider != null)
                aggregate = SnapshotProvider.GetSnapshotOrDefault(key);

            // Generate query for subsequent events to last snap
            var query = GenerateAggregateEventQuery(key, aggregate.RevisionNumber);
            var results = EventStoreTable.ExecuteQuery(query).ToList();

            // Rebuild aggregate current state
            RecomposeAggregateFromEvents(aggregate, results);

            return aggregate;
        }

        /// <summary>
        ///     Load an aggregate by it's key
        /// </summary>
        /// <param name="key">Key value for aggregate</param>
        /// <returns></returns>
        public async Task<TAggregateType> GetAsync(TKeyType key)
        {
            // Stage 1: Most recent snapshot or blank
            var aggregate = GetBlank(key);
            if (SnapshotProvider != null)
                aggregate = await SnapshotProvider.GetSnapshotOrDefaultAsync(key);

            // Generate query for subsequent events to last snap
            var query = GenerateAggregateEventQuery(key, aggregate.RevisionNumber);
            var results = (await EventStoreTable.ExecuteQueryAsync(query)).ToList();

            // Rebuild aggregate current state
            RecomposeAggregateFromEvents(aggregate, results);

            return aggregate;
        }

        /// <summary>
        ///     Put an aggregates pending events into storage.
        /// </summary>
        /// <param name="aggregate">Aggregate</param>
        /// <param name="originalVersion">Original version</param>
        public void Put(TAggregateType aggregate, long originalVersion)
        {
            try
            {
                var currentPosition = originalVersion;

                if (aggregate.PendingEvents.Count > 1)
                {
                    /// Multi-operation case
                    var batch = new TableBatchOperation();
                    foreach (var eventInstance in aggregate.PendingEvents)
                    {
                        currentPosition++;
                        TableEntity newRow = new AzureTableStorageEvent<TAggregateType, TKeyType>(aggregate,
                            eventInstance, currentPosition, long.MaxValue.ToString().Length);
                        batch.Add(TableOperation.Insert(newRow, false));
                    }
                    EventStoreTable.ExecuteBatch(batch);
                }
                else if (aggregate.PendingEvents.Count == 1)
                {
                    // Single operation case
                    currentPosition++;
                    var eventInstance = aggregate.PendingEvents.First();
                    TableEntity newRow = new AzureTableStorageEvent<TAggregateType, TKeyType>(aggregate, eventInstance,
                        currentPosition, long.MaxValue.ToString().Length);
                    EventStoreTable.Execute(TableOperation.Insert(newRow, false));
                }

                // Need snapshot?
                if (SnapshotPolicy != null && SnapshotPolicy.NeedsSnapshot(aggregate, originalVersion))
                    if (SnapshotProvider != null)
                        SnapshotProvider.PutSnapshot(aggregate);

                // Now we've committed, send the events
                foreach (var pending in aggregate.PendingEvents)
                    EventBus.WriteEvent(pending);
            }
            catch (StorageException se)
            {
                throw new ConcurrencyException(se.Message);
            }
        }

        /// <summary>
        ///     Put an aggregates pending events into storage.
        /// </summary>
        /// <param name="aggregate">Aggregate</param>
        /// <param name="originalVersion">Original version</param>
        public async Task PutAsync(TAggregateType aggregate, long originalVersion)
        {
            try
            {
                var currentPosition = originalVersion;

                if (aggregate.PendingEvents.Count > 1)
                {
                    /// Multi-operation case
                    var batch = new TableBatchOperation();
                    foreach (var eventInstance in aggregate.PendingEvents)
                    {
                        currentPosition++;
                        TableEntity newRow = new AzureTableStorageEvent<TAggregateType, TKeyType>(aggregate,
                            eventInstance, currentPosition, long.MaxValue.ToString().Length);
                        batch.Add(TableOperation.Insert(newRow, false));
                    }
                    await EventStoreTable.ExecuteBatchAsync(batch);
                }
                else if (aggregate.PendingEvents.Count == 1)
                {
                    // Single operation case
                    currentPosition++;
                    var eventInstance = aggregate.PendingEvents.First();
                    TableEntity newRow = new AzureTableStorageEvent<TAggregateType, TKeyType>(aggregate, eventInstance,
                        currentPosition, long.MaxValue.ToString().Length);
                    await EventStoreTable.ExecuteAsync(TableOperation.Insert(newRow, false));
                }

                // Need snapshot?
                if (SnapshotPolicy != null && SnapshotPolicy.NeedsSnapshot(aggregate, originalVersion))
                    if (SnapshotProvider != null)
                        await SnapshotProvider.PutSnapshotAsync(aggregate);

                // Now we've committed, send the events
                foreach (var pending in aggregate.PendingEvents)
                    await EventBus.WriteEventAsync(pending);
            }
            catch (StorageException se)
            {
                throw new ConcurrencyException(se.Message);
            }
        }

        /// <summary>
        ///     Create a new blank aggregate
        /// </summary>
        /// <param name="key">Key value</param>
        /// <returns>Blank aggregate with key set appropriately.</returns>
        protected TAggregateType GetBlank(TKeyType key)
        {
            var instance = AggregateFactory.Invoke();
            instance.Key = key;
            return instance;
        }

        /// <summary>
        ///     Create a TableQuery that looks up an aggregate events by key
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="snapshotRevisionNumber">Snapshot revision number</param>
        /// <returns>Query for event stream</returns>
        protected static TableQuery<AzureTableStorageEvent<TAggregateType, TKeyType>> GenerateAggregateEventQuery(
            TKeyType key, long snapshotRevisionNumber)
        {
            var keyPaddingLength = long.MaxValue.ToString().Length;
            var filterString = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, key.KeyString),
                TableOperators.And,
                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThan,
                    snapshotRevisionNumber.ToString().PadLeft(keyPaddingLength, '0'))
                );

            var query = new TableQuery<AzureTableStorageEvent<TAggregateType, TKeyType>>().Where(filterString);
            query.TakeCount = int.MaxValue;
            return query;
        }

        /// <summary>
        ///     Recompose the state of an aggregate from events
        /// </summary>
        /// <param name="aggregate">Initial aggregate (or snapshot)</param>
        /// <param name="results">Results from query to apply</param>
        /// <param name="resolver">Resolver</param>
        protected void RecomposeAggregateFromEvents(TAggregateType aggregate,
            List<AzureTableStorageEvent<TAggregateType, TKeyType>> results)
        {
            var sorted = results.OrderBy(x => x.RowKey);

            var pendingEvents = new List<IEvent>();
            foreach (var eventEntry in sorted)
            {
                var eventType = Context.EventTypes.Where(x => x.Name == eventEntry.EventType).FirstOrDefault();
                var recomposedEvent = eventType.UnpackJsonEvent(eventEntry.EventData, Resolver);
                pendingEvents.Add(recomposedEvent);
            }
            aggregate.ReplayEvents(pendingEvents);
        }
    }
}