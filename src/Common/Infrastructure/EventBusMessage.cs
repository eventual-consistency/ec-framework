using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventualConsistency.Framework.Infrastructure
{
    /// <summary>
    ///     The EventBusMesssage is used as the unit of data exchange between
    ///     classes that leverage the EventBusReader/WriterBase types.
    /// </summary>
    [Serializable]
    public sealed class EventBusMessage
    {
        #region Constructor(s)

        /// <summary>
        ///     Initialize a new, empty EventBusMessage.
        /// </summary>
        public EventBusMessage()
        {

        }

        /// <summary>
        ///     Initialize a new EventBusMessage from an event instance
        /// </summary>
        /// <param name="eventInstance">Event Instance</param>
        public EventBusMessage(IEvent eventInstance)
        {
            // Validate Arguments
            if (eventInstance == null)
                throw new ArgumentNullException(nameof(eventInstance));

            // Build instance
            EventType = eventInstance.GetType().Name;
            EventJson = eventInstance.ToJson();
        }

        /// <summary>
        ///     Initialize a new EventBusMessage
        /// </summary>
        /// <param name="eventType">Event Type</param>
        /// <param name="eventJson">JSON data for event</param>
        public EventBusMessage(string eventType, string eventJson)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(eventType))
                throw new ArgumentNullException(nameof(eventType));
            if (string.IsNullOrWhiteSpace(eventJson))
                throw new ArgumentNullException(nameof(eventJson));

            // Build instance
            EventType = eventType;
            EventJson = eventJson;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Event Type
        /// </summary>
        public string EventType { get; set; }

        /// <summary>
        ///     Event Json
        /// </summary>
        public string EventJson { get; set; }

        #endregion
    }
}
