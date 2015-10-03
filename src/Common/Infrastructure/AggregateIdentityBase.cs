namespace EventualConsistency.Framework.Infrastructure
{
    /// <summary>
    ///     Base type for aggregate identities
    /// </summary>
    /// <typeparam name="TNativeType">Native Type</typeparam>
    public abstract class AggregateIdentityBase<TNativeType>
    {
        /// <summary>
        ///     Initialize
        /// </summary>
        protected AggregateIdentityBase()
        {
            RawValue = default(TNativeType);
        }

        /// <summary>
        ///     Initialize with an identity value
        /// </summary>
        /// <param name="identityValue">Specific value</param>
        protected AggregateIdentityBase(TNativeType identityValue)
        {
            RawValue = identityValue;
        }

        /// <summary>
        ///     Raw Value of the identity
        /// </summary>
        public TNativeType RawValue { get; private set; }

        /// <summary>
        ///     Get the hash code
        /// </summary>
        /// <remarks>
        ///     Uses raw values hash.
        /// </remarks>
        public override int GetHashCode()
        {
            return RawValue.GetHashCode();
        }

        /// <summary>
        ///     Set raw value
        /// </summary>
        /// <param name="rawValue">Raw Value</param>
        protected void SetRawValue(TNativeType rawValue)
        {
            RawValue = rawValue;
        }

        /// <summary>
        ///     Are two values equal?
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns>True if this instance equals obj</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                // If both are null, we're equal
                return RawValue == null;
            }

            var baseIdentity = obj as AggregateIdentityBase<TNativeType>;
            if (baseIdentity != null)
            {
                // Boxed comparisons
                var native = baseIdentity;
                return native.RawValue.Equals(RawValue);
            }
            if (obj is TNativeType)
            {
                // Unbox one side and compare.
                return ((TNativeType) obj).Equals(RawValue);
            }

            return false;
        }

        /// <summary>
        ///     Convert to String
        /// </summary>
        /// <returns>String representation of identity</returns>
        public override string ToString()
        {
            if (RawValue == null)
                return string.Empty;
            return RawValue.ToString();
        }
    }
}