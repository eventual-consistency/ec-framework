using System.Threading.Tasks;

namespace EventualConsistency.Framework
{
    /// <summary>
    ///     Event Bus Writer
    /// </summary>
    public interface IEventBusWriter
    {
        /// <summary>
        ///     Write an event to the event bus
        /// </summary>
        /// <param name="eventInstance">Event instance.</param>
        void WriteEvent(IEvent eventInstance);

        /// <summary>
        ///     Write an event to the event bus
        /// </summary>
        /// <param name="eventInstance">Event instance.</param>
        Task WriteEventAsync(IEvent eventInstance);
    }
}