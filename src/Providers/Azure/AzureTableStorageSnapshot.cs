using System;
using EventualConsistency.Framework;
using EventualConsistency.Framework.Infrastructure;
using Microsoft.WindowsAzure.Storage.Table;

namespace EventualConsistency.Providers.Azure
{
    /// <summary>
    ///     A snapshot stored in azure table storage.
    /// </summary>
    /// <typeparam name="TAggregateType">Aggregate Type</typeparam>
    /// <typeparam name="TKeyType">Key Type</typeparam>
    public class AzureTableStorageSnapshot<TAggregateType, TKeyType> : TableEntity
        where TAggregateType : IKeyedAggregate<TKeyType>
        where TKeyType : IAggregateIdentity
    {
        /// <summary>
        ///     Initialize a new table storage entity
        /// </summary>
        /// <param name="aggregateInstance">Aggregate Instance</param>
        public AzureTableStorageSnapshot(TAggregateType aggregateInstance)
        {
            PartitionKey = aggregateInstance.Key.KeyString;
            RowKey = typeof (TAggregateType).Name + "_" + aggregateInstance.Key.KeyString;
            AggregateState = aggregateInstance.ToJson();
            CreationTimeUtc = DateTime.UtcNow;
        }

        /// <summary>
        ///     Initialize a blank table storage event
        /// </summary>
        public AzureTableStorageSnapshot()
        {
        }

        public string AggregateState { get; set; }

        /// <summary>
        ///     Creation time
        /// </summary>
        public DateTime CreationTimeUtc { get; set; }
    }
}