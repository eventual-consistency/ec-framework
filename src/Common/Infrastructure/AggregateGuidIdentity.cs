using System;

namespace EventualConsistency.Framework.Infrastructure
{
    /// <summary>
    ///     Simple Guiod ID
    /// </summary>
    public class AggregateGuidIdentity : AggregateIdentityBase<Guid>, IAggregateIdentity
    {
        /// <summary>
        ///     Initialize a new instance without a value
        /// </summary>
        public AggregateGuidIdentity()
        {
        }

        /// <summary>
        ///     Initialzie a new instance with a specific value
        /// </summary>
        /// <param name="value"></param>
        public AggregateGuidIdentity(Guid value)
            : base(value)
        {
        }

        /// <summary>
        ///     Identity value as string
        /// </summary>
        public string KeyString
        {
            get { return RawValue.ToString(); }
            set { SetRawValue(Guid.Parse(value)); }
        }

        /// <summary>
        ///     Get the partition value assignment
        /// </summary>
        /// <param name="totalPartitions">Total number of partitions</param>
        /// <returns>Partition number</returns>
        public int GetPartitionAssignment(int totalPartitions)
        {
            return RawValue.GetHashCode()%totalPartitions;
        }

        /// <summary>
        ///     Convert implicity to raw value
        /// </summary>
        /// <param name="identity">Identity value to convert</param>
        /// <returns>Raw value</returns>
        public static implicit operator Guid(AggregateGuidIdentity identity)
        {
            if (identity == null)
                return Guid.Empty;
            return identity.RawValue;
        }

        /// <summary>
        ///     Conver to an identity value
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Identity</returns>
        public static implicit operator AggregateGuidIdentity(Guid value)
        {
            return new AggregateGuidIdentity(value);
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