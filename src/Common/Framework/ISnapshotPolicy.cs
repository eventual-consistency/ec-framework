namespace EventualConsistency.Framework
{
    /// <summary>
    ///     Snapshot policy controller
    /// </summary>
    public interface ISnapshotPolicy
    {
    }

    /// <summary>
    ///     Snapshot Policy interface - Describes operations for classes that
    ///     determine if we need to write a snapshot of an aggregate or not.
    /// </summary>
    /// <typeparam name="TAggregateType">Aggregate Type</typeparam>
    /// <typeparam name="TKeyType">Key Type</typeparam>
    public interface ISnapshotPolicy<TAggregateType, TKeyType> : ISnapshotPolicy
        where TAggregateType : IKeyedAggregate<TKeyType>
        where TKeyType : IAggregateIdentity
    {
        /// <summary>
        ///     Do we need a snapshot of the specified instance?
        /// </summary>
        /// <param name="aggregateInstance">Aggregate instance</param>
        /// <param name="lastSnapshotRevision">Last snapshot revision number</param>
        /// <returns>True if we need a snapshot, false otherwise.</returns>
        bool NeedsSnapshot(TAggregateType aggregateInstance, long lastSnapshotRevision);
    }
}