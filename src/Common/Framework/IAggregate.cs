using System.Collections.Generic;

namespace EventualConsistency.Framework
{
    /// <summary>
    ///     Common interface and behavior definition for aggregates.
    /// </summary>
    public interface IAggregate
    {
        /// <summary>
        ///     Pending events on the aggregate.
        /// </summary>
        /// <value>The pending events.</value>
        /// <remarks>
        ///     The pending events represent events that were 'Applied' to the
        ///     aggregate in response to succesful CommandHandlers.
        /// </remarks>
        IReadOnlyList<IEvent> PendingEvents { get; }

        /// <summary>
        ///     The revision number of the aggregate in the event store. This allows us
        ///     to perform optimistic concurrency behaviors.
        /// </summary>
        /// <value>The sequence number.</value>
        long RevisionNumber { get; }

        /// <summary>
        ///     Rebuild the aggregate state by applying events.
        /// </summary>
        /// <param name="eventsToReplay">Events to replay.</param>
        void ReplayEvents(IEnumerable<IEvent> eventsToReplay);
    }
}