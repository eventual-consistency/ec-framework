using System;
using EventualConsistency.Framework;
using EventualConsistency.Framework.Infrastructure;
using Microsoft.WindowsAzure.Storage.Table;

namespace EventualConsistency.Providers.Azure
{
    /// <summary>
    ///     An event stored in azure table storage.
    /// </summary>
    /// <typeparam name="TAggregateType">Aggregate Type</typeparam>
    /// <typeparam name="TKeyType">Key Type</typeparam>
    public class AzureTableStorageEvent<TAggregateType, TKeyType> : TableEntity
        where TAggregateType : IKeyedAggregate<TKeyType>
        where TKeyType : IAggregateIdentity
    {
        /// <summary>
        ///     Initialize a new table storage entity
        /// </summary>
        /// <param name="aggregateInstance">Aggregate Instance</param>
        /// <param name="eventToStore">Event to Store</param>
        /// <param name="sequenceNumber">Sequence number for event</param>
        /// <param name="keyPaddingLength">Event sequence number padding value</param>
        public AzureTableStorageEvent(TAggregateType aggregateInstance, IEvent eventToStore, long sequenceNumber,
            int keyPaddingLength)
        {
            // Validate Arguments
            if (aggregateInstance == null)
                throw new ArgumentNullException(nameof(aggregateInstance));
            if (eventToStore == null)
                throw new ArgumentNullException(nameof(eventToStore));

            PartitionKey = aggregateInstance.Key.KeyString;
            RowKey = sequenceNumber.ToString().PadLeft(keyPaddingLength, '0');
            EventType = eventToStore.GetType().Name;
            EventData = eventToStore.ToJson();
        }

        /// <summary>
        ///     Initialize a blank table storage event
        /// </summary>
        public AzureTableStorageEvent()
        {
        }

        /// <summary>
        ///     Event Type
        /// </summary>
        public string EventType { get; set; }

        public string EventData { get; set; }
    }
}