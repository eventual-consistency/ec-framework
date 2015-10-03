using System;
using System.Runtime.Serialization;

namespace EventualConsistency.Framework
{
    /// <summary>
    ///     A ConcurrencyException occurs when an event store tries to update an aggregate and the underlying
    ///     aggregate has changed in the interim. In this scenario it is expected that the command bus retries
    ///     the event a suitable number of times.
    /// </summary>
    [Serializable]
    public class ConcurrencyException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:ConcurrencyException" /> class
        /// </summary>
        public ConcurrencyException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:ConcurrencyException" /> class
        /// </summary>
        /// <param name="message">A <see cref="T:System.String" /> that describes the exception. </param>
        public ConcurrencyException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:ConcurrencyException" /> class
        /// </summary>
        /// <param name="message">A <see cref="T:System.String" /> that describes the exception. </param>
        /// <param name="inner">The exception that is the cause of the current exception. </param>
        public ConcurrencyException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:ConcurrencyException" /> class
        /// </summary>
        /// <param name="context">The contextual information about the source or destination.</param>
        /// <param name="info">The object that holds the serialized object data.</param>
        protected ConcurrencyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}