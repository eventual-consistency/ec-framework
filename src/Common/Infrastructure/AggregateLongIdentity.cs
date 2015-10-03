namespace EventualConsistency.Framework.Infrastructure
{
    /// <summary>
    ///     Simple Integer ID
    /// </summary>
    public class AggregateLongIdentity : AggregateIdentityBase<long>
    {
        /// <summary>
        ///     Initialize a new instance without a value
        /// </summary>
        public AggregateLongIdentity()
        {
        }

        /// <summary>
        ///     Initialzie a new instance with a specific value
        /// </summary>
        /// <param name="value">Identity value</param>
        public AggregateLongIdentity(long value)
            : base(value)
        {
        }

        /// <summary>
        ///     Identity valu
        public string KeyString
        {
            get { return RawValue.ToString(); }
            set { SetRawValue(long.Parse(value)); }
        }

        /// <summary>
        ///     Get the partition value assignment
        /// </summary>
        /// <param name="totalPartitions">Total number of partitions</param>
        /// <returns>Partition number</returns>
        public int GetPartitionAssignment(int totalPartitions)
        {
            // This cast isn't particularly evil, because N % M will always be an integer
            // for any N where M is an integer, so casting away the long is always valid, 
            // mathematically.
            return (int) (RawValue%totalPartitions);
        }

        /// <summary>
        ///     Convert implicity to raw value
        /// </summary>
        /// <param name="identity">Identity value to convert</param>
        /// <returns>Raw value</returns>
        public static implicit operator long(AggregateLongIdentity identity)
        {
            return identity.RawValue;
        }

        /// <summary>
        ///     Conver to an identity value
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Identity</returns>
        public static implicit operator AggregateLongIdentity(long value)
        {
            return new AggregateLongIdentity(value);
        }

        /// <summary>
        ///     Convert to string
        /// </summary>
        /// <returns>String value</returns>
        public override string ToString()
        {
            return RawValue.ToString();
        }
    }
}