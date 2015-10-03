using System;
using System.Runtime.Serialization;

namespace EventualConsistency.Framework
{
    /// <summary>
    ///     Unknown Command Exception
    /// </summary>
    [Serializable]
    public class UnknownCommandException : Exception
    {
        public UnknownCommandException()
        {
        }

        public UnknownCommandException(string message) : base(message)
        {
        }

        public UnknownCommandException(string message, Exception inner) : base(message, inner)
        {
        }

        protected UnknownCommandException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}