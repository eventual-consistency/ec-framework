using System.Threading.Tasks;

namespace EventualConsistency.Framework
{
    /// <summary>
    ///     Asychronous event handler
    /// </summary>
    /// <typeparam name="TEventType">Event Type</typeparam>
    public interface IEventHandlerAsync<TEventType>
        where TEventType : IEvent
    {
        /// <summary>
        ///     Handle the specified instance of the event asynchronously
        /// </summary>
        /// <param name="eventInstance">Event Instance</param>
        /// <returns>Continuation token</returns>
        Task HandleAsync(TEventType eventInstance);
    }
}