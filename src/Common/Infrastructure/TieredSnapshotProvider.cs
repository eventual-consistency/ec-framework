using System;
using System.Threading.Tasks;

namespace EventualConsistency.Framework.Infrastructure
{
    /// <summary>
    ///     The TieredSnapshotProvider writes snapshots to multiple locations and performs a tiered fetch.
    ///     This allows us to avoid many I/O operations on back end snapshot providers for complex aggregates
    ///     with long histories.
    /// </summary>
    /// <remarks>
    ///     Example usage:
    ///     MemorySnapshotProvider              - Every 10 events
    ///     AzureTableStorageSnapshotProvider   - Every 100 events
    /// </remarks>
    public class TieredSnapshotProvider<TAggregateType, TKeyType> : ISnapshotProvider<TAggregateType, TKeyType>
        where TAggregateType : IKeyedAggregate<TKeyType>, new()
        where TKeyType : IAggregateIdentity
    {
        public TAggregateType GetSnapshotOrDefault(TKeyType key)
        {
            throw new NotImplementedException();
        }

        public Task<TAggregateType> GetSnapshotOrDefaultAsync(TKeyType key)
        {
            throw new NotImplementedException();
        }

        public void PutSnapshot(TAggregateType aggregate)
        {
            throw new NotImplementedException();
        }

        public Task PutSnapshotAsync(TAggregateType aggregate)
        {
            throw new NotImplementedException();
        }
    }
}