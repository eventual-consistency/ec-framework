namespace EventualConsistency.Framework.Infrastructure
{
    /// <summary>
    ///     Simple Integer ID
    /// </summary>
    public class AggregateIntegerIdentity : AggregateIdentityBase<int>, IAggregateIdentity
    {
        /// <summary>
        ///     Initialize a new instance without a value
        /// </summary>
        public AggregateIntegerIdentity()
        {
        }

        /// <summary>
        ///     Initialzie a new instance with a specific value
        /// </summary>
        /// <param name="value"></param>
        public AggregateIntegerIdentity(int value) : base(value)
        {
        }

        /// <summary>
        ///     Identity value as string
        /// </summary>
        public string KeyString
        {
            get { return RawValue.ToString(); }
            set { SetRawValue(int.Parse(value)); }
        }

        /// <summary>
        ///     Get the partition value assignment
        /// </summary>
        /// <param name="totalPartitions">Total number of partitions</param>
        /// <returns>Partition number</returns>
        public int GetPartitionAssignment(int totalPartitions)
        {
            return RawValue%totalPartitions;
        }

        /// <summary>
        ///     Convert implicity to raw value
        /// </summary>
        /// <param name="identity">Identity value to convert</param>
        /// <returns>Raw value</returns>
        public static implicit operator int(AggregateIntegerIdentity identity)
        {
            if (identity == null)
                return 0;
            return identity.RawValue;
        }

        /// <summary>
        ///     Conver to an identity value
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Identity</returns>
        public static implicit operator AggregateIntegerIdentity(int value)
        {
            return new AggregateIntegerIdentity(value);
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