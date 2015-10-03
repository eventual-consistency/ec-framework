using System;
using System.Linq;
using System.Threading.Tasks;
using EventualConsistency.Framework;
using EventualConsistency.Framework.Infrastructure;
using Microsoft.WindowsAzure.Storage.Table;

namespace EventualConsistency.Providers.Azure
{
    /// <summary>
    ///     Azure Table Storage snapshot provider. Stores snapshots in a specified CloudTable instance
    ///     and allows for simple retrieval.
    /// </summary>
    /// <typeparam name="TAggregateType">Aggregate Type</typeparam>
    /// <typeparam name="TKeyType">Key Type</typeparam>
    public class AzureTableStorageSnapshotProvider<TAggregateType, TKeyType> :
        ISnapshotProvider<TAggregateType, TKeyType>
        where TAggregateType : class, IKeyedAggregate<TKeyType>
        where TKeyType : IAggregateIdentity
    {
        #region Constructor(s)

        /// <summary>
        ///     Initialize a new instance of the snapshot table provider.
        /// </summary>
        /// <param name="aggregateFactory">Aggregate factory method</param>
        /// <param name="snapshotTable">Snapshot Table</param>
        /// <param name="resolver">Type Resolver</param>
        public AzureTableStorageSnapshotProvider(Func<TAggregateType> aggregateFactory, CloudTable snapshotTable,
            ITypeResolver resolver)
        {
            AggregateFactory = aggregateFactory;
            SnapshotTable = snapshotTable;
            Resolver = resolver;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Cloud Table
        /// </summary>
        protected CloudTable SnapshotTable { get; }

        /// <summary>
        ///     Aggregate Factory
        /// </summary>
        protected Func<TAggregateType> AggregateFactory { get; }

        /// <summary>
        ///     Type Resolver
        /// </summary>
        protected ITypeResolver Resolver { get; }

        #endregion

        #region ISnapshotProvider Implementation

        /// <summary>
        ///     Get the current snapshot of the aggregate (or a blank instance)
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Aggregate type or blank instance</returns>
        public TAggregateType GetSnapshotOrDefault(TKeyType key)
        {
            var query = GetSnapshotQuery(key);
            var result = SnapshotTable.ExecuteQuery(query).FirstOrDefault();
            if (result == null)
            {
                var newInstance = AggregateFactory.Invoke();
                newInstance.Key = key;
                return newInstance;
            }

            var aggregate = JsonToAggregateType(result.AggregateState, Resolver);
            return aggregate;
        }

        /// <summary>
        ///     Convert a JSON string to the specified aggregate type for this class.
        /// </summary>
        /// <param name="aggregateState">Snapshot JSON string</param>
        /// <param name="resolver">Type Resolver</param>
        /// <returns></returns>
        private static TAggregateType JsonToAggregateType(string aggregateState, ITypeResolver resolver)
        {
            return aggregateState.UnpackJson<TAggregateType>(resolver);
        }

        /// <summary>
        ///     Get the current snapshot of the a ggregate (or a blank instance)
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Aggregate type or blank instance</returns>
        public async Task<TAggregateType> GetSnapshotOrDefaultAsync(TKeyType key)
        {
            var query = GetSnapshotQuery(key);
            var result = (await SnapshotTable.ExecuteQueryAsync(query)).FirstOrDefault();
            if (result == null)
            {
                var newInstance = AggregateFactory.Invoke();
                newInstance.Key = key;
                return newInstance;
            }

            var aggregate = JsonToAggregateType(result.AggregateState, Resolver);
            return aggregate;
        }

        /// <summary>
        ///     Write the snapshot state of the aggregate.
        /// </summary>
        /// <param name="aggregate">Aggregate</param>
        public void PutSnapshot(TAggregateType aggregate)
        {
            TableEntity newRow = new AzureTableStorageSnapshot<TAggregateType, TKeyType>(aggregate);
            SnapshotTable.Execute(TableOperation.InsertOrReplace(newRow));
        }

        /// <summary>
        ///     Write the snapshot state of the aggregate.
        /// </summary>
        /// <param name="aggregate">Aggregate</param>
        public async Task PutSnapshotAsync(TAggregateType aggregate)
        {
            TableEntity newRow = new AzureTableStorageSnapshot<TAggregateType, TKeyType>(aggregate);
            await SnapshotTable.ExecuteAsync(TableOperation.InsertOrReplace(newRow));
        }

        /// <summary>
        ///     Get a query that looks up the specified aggregate by key.
        /// </summary>
        /// <param name="key">Snapshot Key</param>
        /// <returns>Aggregate lookup query</returns>
        protected TableQuery<AzureTableStorageSnapshot<TAggregateType, TKeyType>> GetSnapshotQuery(TKeyType key)
        {
            return new TableQuery<AzureTableStorageSnapshot<TAggregateType, TKeyType>>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, key.KeyString)
                );
        }

        #endregion
    }
}