namespace EventualConsistency.Framework
{
    /// <summary>
    ///     Aggregates with a unique identifier/key.
    /// </summary>
    public interface IKeyedAggregate<TKeyType> : IAggregate
        where TKeyType : IAggregateIdentity
    {
        /// <summary>
        ///     Aggregate Key
        /// </summary>
        TKeyType Key { get; set; }
    }
}