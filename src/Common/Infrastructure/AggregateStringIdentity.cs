namespace EventualConsistency.Framework.Infrastructure
{
    /// <summary>
    ///     AggregateStringIdentity stores a lowercased string as the aggregate identity.
    /// </summary>
    public class AggregateStringIdentity : AggregateIdentityBase<string>, IAggregateIdentity
    {
        /// <summary>
        ///     Initialize a new instance without a value
        /// </summary>
        public AggregateStringIdentity()
        {
        }

        /// <summary>
        ///     Initialzie a new instance with a specific value
        /// </summary>
        /// <param name="value"></param>
        public AggregateStringIdentity(string value)
            : base(value)
        {
        }

        /// <summary>
        ///     Identity value as string
        /// </summary>
        public string KeyString
        {
            get { return RawValue; }
            set { SetRawValue(value.ToLowerInvariant()); }
        }

        /// <summary>
        ///     Get the partition value assignment
        /// </summary>
        /// <param name="totalPartitions">Total number of partitions</param>
        /// <returns>Partition number</returns>
        public int GetPartitionAssignment(int totalPartitions)
        {
            return (RawValue ?? string.Empty).GetHashCode()%totalPartitions;
        }

        /// <summary>
        ///     Convert implicity to raw value
        /// </summary>
        /// <param name="identity">Identity value to convert</param>
        /// <returns>Raw value</returns>
        public static implicit operator string(AggregateStringIdentity identity)
        {
            return identity.RawValue;
        }

        /// <summary>
        ///     Conver to an identity value
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Identity</returns>
        public static implicit operator AggregateStringIdentity(string value)
        {
            return new AggregateStringIdentity(value);
        }

        /// <summary>
        ///     Convert to string
        /// </summary>
        /// <returns>String value</returns>
        public override string ToString()
        {
            return RawValue;
        }
    }
}