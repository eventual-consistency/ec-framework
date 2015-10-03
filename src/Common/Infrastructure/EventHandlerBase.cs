namespace EventualConsistency.Framework.Infrastructure
{
    /// <summary>
    ///     Base class for event handler logic
    /// </summary>
    /// <typeparam name="TEventType">Event Type</typeparam>
    public abstract class EventHandlerBase<TEventType> : IEventHandler<TEventType>
        where TEventType : class, IEvent
    {
        /// <summary>
        ///     Handle the specified instance of the event.
        /// </summary>
        /// <param name="eventInstance">Event instance.</param>
        public abstract void Handle(TEventType eventInstance);
    }
}