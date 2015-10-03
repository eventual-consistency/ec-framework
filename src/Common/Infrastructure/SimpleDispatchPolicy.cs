using System;

namespace EventualConsistency.Framework.Infrastructure
{
    /// <summary>
    ///     The Simple policy simply executes the action on the current thread. This is useful
    ///     for small-scale applications where you want deterministic threading behavior.
    /// </summary>
    public class SimpleDispatchPolicy : IDispatchPolicy
    {
        #region IDispatchPolicy implementation

        /// <summary>
        ///     Dispatch the event
        /// </summary>
        /// <param name="action">Action to dispatch.</param>
        public void Dispatch(Action action)
        {
            if (action != null)
                action();
        }

        #endregion
    }
}