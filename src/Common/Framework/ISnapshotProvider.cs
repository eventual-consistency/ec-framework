using System.Threading.Tasks;

namespace EventualConsistency.Framework
{
    /// <summary>
    ///     Non-generic base interface for Snapshot provider implementations.
    /// </summary>
    public interface ISnapshotProvider
    {
    }

    /// <summary>
    ///     Snapshot Provider interface - Describes operations for classes that
    ///     generate and recover snapshots of aggregates.
    /// </summary>
    /// <typeparam name="TAggregateType">Aggregate Type</typeparam>
    /// <typeparam name="TKeyType">Key Type</typeparam>
    public interface ISnapshotProvider<TAggregateType, TKeyType> : ISnapshotProvider
        where TAggregateType : IKeyedAggregate<TKeyType>
        where TKeyType : IAggregateIdentity
    {
        /// <summary>
        ///     Get the current snapshot of the aggregate (or a blank instance)
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Aggregate type or blank instance</returns>
        TAggregateType GetSnapshotOrDefault(TKeyType key);

        /// <summary>
        ///     Get the current snapshot of the aggregate (or a blank instance)
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Aggregate type or blank instance</returns>
        Task<TAggregateType> GetSnapshotOrDefaultAsync(TKeyType key);

        /// <summary>
        ///     Write the snapshot state of the aggregate.
        /// </summary>
        /// <param name="aggregate">Aggregate</param>
        void PutSnapshot(TAggregateType aggregate);

        /// <summary>
        ///     Write the snapshot state of the aggregate.
        /// </summary>
        /// <param name="aggregate">Aggregate</param>
        Task PutSnapshotAsync(TAggregateType aggregate);
    }
}