using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EventualConsistency.Integration.Nancy
{
    /// <summary>
    ///     The NancyEcfTypeResolutionException is thrown when a type cannot be resolved at runtime.
    /// </summary>
    [Serializable]
    public class NancyEcfTypeResolutionException : Exception
    {
        #region Constructor(s)

        /// <summary>
        ///     Initialize a NancyEcfTypeResolutionException with no parameters
        /// </summary>
        public NancyEcfTypeResolutionException()
        {
        }

        /// <summary>
        ///     Initialize a NancyEcfTypeResolutionException with a localized message.
        /// </summary>
        /// <param name="message">Message text</param>
        public NancyEcfTypeResolutionException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Initialize a NancyEcfTypeResolutionException with a localized message and an inner exception.
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="inner">Inner exception</param>
        public NancyEcfTypeResolutionException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        ///     Initialize a NancyEcfTypeResolutionException from a deserialization context
        /// </summary>
        /// <param name="info">Serialization information</param>
        /// <param name="context">Streaming context</param>
        protected NancyEcfTypeResolutionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        #endregion
    }
}
