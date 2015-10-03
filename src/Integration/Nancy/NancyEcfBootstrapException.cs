using System;
using System.Runtime.Serialization;

namespace EventualConsistency.Integration.Nancy
{
    /// <summary>
    ///     The NancyEcfBootstrapException is thrown when ECF is being configured for use with Nancy and it's inbuilt
    ///     TinyIoC implementation and something has gone wrong, such as ambiguous resolution of critical types.
    /// </summary>
    [Serializable]
    public class NancyEcfBootstrapException : Exception
    {
        #region Constructor(s)

        /// <summary>
        ///     Initialize a NancyEcfBootstrapException with no parameters
        /// </summary>
        public NancyEcfBootstrapException()
        {
        }

        /// <summary>
        ///     Initialize a NancyEcfBootstrapException with a localized message.
        /// </summary>
        /// <param name="message">Message text</param>
        public NancyEcfBootstrapException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Initialize a NancyEcfBootstrapException with a localized message and an inner exception.
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="inner">Inner exception</param>
        public NancyEcfBootstrapException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        ///     Initialize a NancyEcfBootstrapException from a deserialization context
        /// </summary>
        /// <param name="info">Serialization information</param>
        /// <param name="context">Streaming context</param>
        protected NancyEcfBootstrapException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        #endregion
    }
}