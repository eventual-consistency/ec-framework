namespace EventualConsistency.Framework
{
    /// <summary>
    ///     Non-Generic base interface for event handlers.
    /// </summary>
    public interface IEventHandler
    {
    }

    /// <summary>
    ///     Generic interface for all event handler implementations
    /// </summary>
    /// <typeparam name="TEventType">Event Type</typeparam>
    public interface IEventHandler<TEventType> : IEventHandler
        where TEventType : IEvent
    {
        /// <summary>
        ///     Handle the specified instance of the event.
        /// </summary>
        /// <param name="eventInstance">Event instance.</param>
        void Handle(TEventType eventInstance);
    }
}