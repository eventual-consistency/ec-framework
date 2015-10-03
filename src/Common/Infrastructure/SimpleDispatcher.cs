using System;
using System.Collections.Generic;
using Polygamy;

namespace EventualConsistency.Framework.Infrastructure
{
    /// <summary>
    ///     The SimpleDispatcher is a dispatcher for IEvents that finds the suitable IEventHandler(Of TEventType)
    ///     and calls Handle.
    /// </summary>
    [PolymorphicDispatchControl(typeof (IEventHandler<>), "Handle")]
    public class SimpleDispatcher : PolymorphicDispatcherBase<IEventHandler, IEvent>
    {
        #region Constructor(s)

        /// <summary>
        ///     Initialize a new SimpleDispatcher with a set of handler types
        /// </summary>
        /// <param name="handlers">Handlers</param>
        public SimpleDispatcher(IEnumerable<IEventHandler> handlers) : base(handlers)
        {
            // No work to do here
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Handle a call to Dispatch that has no handler defined.
        /// </summary>
        /// <param name="instanceToDispatch">Instance to dispatch</param>
        /// <param name="dispatcherType">Dispatcher type</param>
        protected override void HandleUndefinedDispatch(IEvent instanceToDispatch, Type dispatcherType)
        {
            // Do nothing here.
        }

        #endregion
    }
}