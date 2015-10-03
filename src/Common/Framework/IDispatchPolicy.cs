using System;

namespace EventualConsistency.Framework
{
    /// <summary>
    ///     Dispatch Policy for executing events. Used by in-memory/process variations of
    ///     infrastructure to determine how to deal with multicast scenarios.
    /// </summary>
    public interface IDispatchPolicy
    {
        /// <summary>
        ///     Dispatch the event
        /// </summary>
        /// <param name="action">Action to dispatch.</param>
        void Dispatch(Action action);
    }
}