using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EventualConsistency.Framework.Infrastructure
{
    /// <summary>
    ///     Base type for aggregates
    /// </summary>
    public abstract class AggregateBase
    {
        /// <summary>
        ///     Aggregate Id
        /// </summary>
        public IAggregateIdentity Identity { get; protected set; }

        /// <summary>
        ///     State Object
        /// </summary>
        public IStateObject StateObject { get; protected set; }

        /// <summary>
        ///     Revision Number
        /// </summary>
        public long RevisionNumber { get; protected set; }
    }

    /// <summary>
    ///     Base type for aggregates that implements most of the boilerplate logic and capabilities.
    /// </summary>
    [Serializable]
    [JsonConverter(typeof (AggregateBaseJsonConverter))]
    public abstract class AggregateBase<TKeyType, TStateType> : AggregateBase, IKeyedAggregate<TKeyType>
        where TStateType : class, IStateObject, new()
        where TKeyType : class, IAggregateIdentity
    {
        #region Private Fields

        /// <summary>
        ///     Pending events to commit to store since we were loaded.
        /// </summary>
        [NonSerialized] private readonly List<IEvent> _pendingEvents;

        #endregion

        #region Constructor(s)

        /// <summary>
        ///     Initialize a new, blank instance of the type.
        /// </summary>
        protected AggregateBase()
        {
            _pendingEvents = new List<IEvent>();
            StateObject = new TStateType();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     State Object
        /// </summary>
        /// <value>The state object.</value>
        public new TStateType StateObject
        {
            get { return base.StateObject as TStateType; }
            set { base.StateObject = value; }
        }

        #endregion

        #region IKeyedAggregate implementation

        /// <summary>
        ///     Unique aggregate key
        /// </summary>
        /// <value>The key.</value>
        public TKeyType Key
        {
            get { return Identity as TKeyType; }
            set { Identity = value; }
        }

        /// <summary>
        ///     The revision number of the aggregate in the event store. This allows us
        ///     to perform optimistic concurrency behaviors.
        /// </summary>
        /// <value>The sequence number.</value>
        public new long RevisionNumber
        {
            get { return base.RevisionNumber; }
            set { base.RevisionNumber = value; }
        }

        #endregion

        #region IAggregate implementation

        /// <summary>
        ///     Called during recovery in order to apply an event to an aggregate.
        /// </summary>
        /// <remarks>
        ///     The events in question should be passed only from the event store, and
        ///     represent the steps required to rebuild the state of the aggregate.
        /// </remarks>
        /// <param name="eventsToReplay">Events to replay.</param>
        public void ReplayEvents(IEnumerable<IEvent> eventsToReplay)
        {
            // Apply each event to the model in sequence.
            foreach (var eventInstance in eventsToReplay)
            {
                MutationCache<TStateType>.ApplyEvent(StateObject, eventInstance);

                // Each event increments the revision.
                RevisionNumber++;
            }
        }

        /// <summary>
        ///     Apply an event to the aggregate.
        /// </summary>
        /// <param name="eventToApply">Event to apply.</param>
        /// <typeparam name="TEventType">The 1st type parameter.</typeparam>
        public void Apply<TEventType>(TEventType eventToApply) where TEventType : IEvent
        {
            // Execute mutation
            MutationCache<TStateType>.ApplyEvent(StateObject, eventToApply);

            // Add to pending list
            _pendingEvents.Add(eventToApply);

            // We're now a higher revision number in memory.
            RevisionNumber++;
        }

        /// <summary>
        ///     Events that have been Apply()'d to this aggregate since it was fetched.
        /// </summary>
        /// <value>The pending events.</value>
        /// <remarks>
        ///     The pending events represent events that were 'Applied' to the
        ///     aggregate in response to succesful CommandHandlers.
        /// </remarks>
        [JsonIgnore]
        public IReadOnlyList<IEvent> PendingEvents => _pendingEvents.AsReadOnly();

        #endregion
    }
}