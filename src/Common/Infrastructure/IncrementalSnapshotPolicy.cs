namespace EventualConsistency.Framework.Infrastructure
{
    /// <summary>
    ///     Incremental snapshot - take a snapshot every N events.
    /// </summary>
    /// <typeparam name="TAggregateType">Aggregate Type</typeparam>
    /// <typeparam name="TKeyType">Key Type</typeparam>
    public class IncrementalSnapshotPolicy<TAggregateType, TKeyType> : ISnapshotPolicy<TAggregateType, TKeyType>
        where TAggregateType : class, IKeyedAggregate<TKeyType>
        where TKeyType : IAggregateIdentity
    {
        /// <summary>
        ///     Number of aggregate events that can occur between snapshots.
        /// </summary>
        /// <param name="revisionsPerSnap">Revisions per snapshot</param>
        public IncrementalSnapshotPolicy(int revisionsPerSnap)
        {
            RevisionsPerSnapshot = revisionsPerSnap;
        }

        /// <summary>
        ///     Revisions per snapshot
        /// </summary>
        protected int RevisionsPerSnapshot { get; }

        /// <summary>
        ///     Do we need a snapshot of the specified instance?
        /// </summary>
        /// <param name="aggregateInstance">Aggregate instance</param>
        /// <param name="lastSnapshotRevision">Last snapshot revision number</param>
        /// <returns>True if we need a snapshot, false otherwise.</returns>
        public bool NeedsSnapshot(TAggregateType aggregateInstance, long lastSnapshotRevision)
        {
            // We should snap at last + increment
            var snapshotAfter = lastSnapshotRevision + RevisionsPerSnapshot;

            // If we're past that point, we need a snap. Also, force a snap exactly on a multiple of the increment
            return (snapshotAfter <= aggregateInstance.RevisionNumber)
                   ||
                   (aggregateInstance.RevisionNumber%RevisionsPerSnapshot == 0);
        }
    }
}