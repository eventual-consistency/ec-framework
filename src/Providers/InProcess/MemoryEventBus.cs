using System.Collections.Generic;
using System.Threading.Tasks;
using EventualConsistency.Framework;

namespace EventualConsistency.Providers.InProcess
{
    /// <summary>
    ///     The MemoryEventBus is a simple event bus that is essentially stateless between invocations and
    ///     lives purely in-process.
    /// </summary>
    public class MemoryEventBus : IEventBusWriter
    {
        /// <summary>
        ///     Initialize a new instance of the MemoryEventBus
        /// </summary>
        public MemoryEventBus(IDispatchPolicy dispatchPolicy)
        {
            Subscribers = new List<MemoryEventBusSubscription>();
            DispatchPolicy = dispatchPolicy;
        }

        public class MemoryEventBusSubscription
        {
            public void NotifyEvent(IEvent eventInstance)
            {
            }
        }

        #region IEventBusWriter implementation

        /// <summary>
        ///     Write an event synchronously to the memory event bus.
        /// </summary>
        /// <param name="eventInstance">Event instance.</param>
        public void WriteEvent(IEvent eventInstance)
        {
            // Notify the event to the subscribers
            foreach (var subscription in Subscribers)
            {
                DispatchPolicy.Dispatch(() => { subscription.NotifyEvent(eventInstance); });
            }
        }

        /// <summary>
        ///     Write an event asynchronously to the memory event bus.
        /// </summary>
        /// <param name="eventInstance">Event instance.</param>
        public async Task WriteEventAsync(IEvent eventInstance)
        {
            // Notify the event to the subscribers
            foreach (var subscription in Subscribers)
            {
                await Task.Run(() =>
                    DispatchPolicy.Dispatch(() => { subscription.NotifyEvent(eventInstance); })
                    );
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Subscribers to this event bus
        /// </summary>
        /// <value>The subscribers.</value>
        protected List<MemoryEventBusSubscription> Subscribers { get; }

        /// <summary>
        ///     Dispatch Policy handler
        /// </summary>
        /// <value>The dispatch policy.</value>
        protected IDispatchPolicy DispatchPolicy { get; }

        #endregion
    }
}