namespace EventualConsistency.Framework
{
    /// <summary>
    ///     The EventBusReader defines a class that listens on an event bus for incoming
    ///     messages and hands them to event handlers.
    /// </summary>
    public interface IEventBusReader
    {
        /// <summary>
        ///     Start listening to the message bus
        /// </summary>
        void Start();

        /// <summary>
        ///     Stop listening
        /// </summary>
        void Stop();
    }
}