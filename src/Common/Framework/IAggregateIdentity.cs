namespace EventualConsistency.Framework
{
    /// <summary>
    ///     Aggregate Identity
    /// </summary>
    public interface IAggregateIdentity
    {
        /// <summary>
        ///     Key String
        /// </summary>
        string KeyString { get; set; }

        /// <summary>
        ///     Get the partition that this key belongs to amongst N partitions.
        /// </summary>
        /// <param name="totalPartitions">Total Partitions</param>
        /// <returns>0 Based partition index</returns>
        /// <remarks>
        ///     Used when manually sharding data.
        /// </remarks>
        int GetPartitionAssignment(int totalPartitions);
    }
}