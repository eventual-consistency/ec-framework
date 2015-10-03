using System;
using System.Collections.Generic;

namespace EventualConsistency.Framework
{
    /// <summary>
    ///     Behaviors of a Bounded Context
    /// </summary>
    public interface IBoundedContext
    {
        /// <summary>
        ///     Name of the bounded context
        /// </summary>
        string ContextName { get; }

        /// <summary>
        ///     Command Types
        /// </summary>
        IEnumerable<Type> CommandTypes { get; }

        /// <summary>
        ///     Event Types
        /// </summary>
        IEnumerable<Type> EventTypes { get; }

        /// <summary>
        ///     Faults
        /// </summary>
        IEnumerable<Type> FaultTypes { get; }
    }
}