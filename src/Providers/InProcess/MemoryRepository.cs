using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventualConsistency.Framework;

namespace EventualConsistency.Providers.InProcess
{
    /// <summary>
    ///     Memory Repository
    /// </summary>
    /// <remarks>
    ///     This uses an internal memory lock for the event store when fetching or popping aggregates, this is because
    ///     whilst concurrent variations of the dictionary class exist, the concurrency guarantees do not apply to
    ///     actions against value types, and we don't want to enforce that TKeyType is a 'class' (i.e. So we could
    ///     lock on the key rather than per-aggregate type).
    /// </remarks>
    public class MemoryRepository<TAggregateType, TKeyType> : IRepository<TAggregateType, TKeyType>
        where TAggregateType : class, IKeyedAggregate<TKeyType>
        where TKeyType : IAggregateIdentity
    {
        #region Private Fields

        private readonly Dictionary<TKeyType, List<IEvent>> m_EventStorage;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initialize a new instance of the memory repository
        /// </summary>
        /// <param name="aggregateFactory">IoC Container</param>
        /// <param name="eventWriter">Event Writer</param>
        public MemoryRepository(Func<TAggregateType> aggregateFactory, IEventBusWriter eventWriter)
        {
            AggregateFactory = aggregateFactory;
            m_EventStorage = new Dictionary<TKeyType, List<IEvent>>();
            EventWriter = eventWriter;
        }

        #endregion

        public Task<TAggregateType> GetAsync(TKeyType key)
        {
            throw new NotImplementedException();
        }

        public Task PutAsync(TAggregateType aggregate, long originalVersion)
        {
            throw new NotImplementedException();
        }

        #region Properties

        /// <summary>
        ///     Event Writer
        /// </summary>
        /// <value>The event writer.</value>
        protected IEventBusWriter EventWriter { get; }

        /// <summary>
        ///     Dependency Resolver
        /// </summary>
        protected Func<TAggregateType> AggregateFactory { get; }

        #endregion

        #region IRepository implementation

        /// <summary>
        ///     Get an instance of the specified aggregate
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>Aggregate instance or null</returns>
        public TAggregateType Get(TKeyType key)
        {
            var result = AggregateFactory.Invoke();
            result.Key = key;
            // Replay any events
            if (m_EventStorage.ContainsKey(key))
                lock (m_EventStorage)
                    result.ReplayEvents(m_EventStorage[key]);

            // We're done.
            return result;
        }

        /// <summary>
        ///     Put the new events that have occured to an aggregate into the store.
        /// </summary>
        /// <param name="aggregate">Aggregate with new events.</param>
        /// <param name="originalVersion">Original version number to change against.</param>
        /// <remarks>Returning without an exception indicates success.</remarks>
        /// <exception cref="EventualConsistency.Framework.ConcurrencyException">
        ///     The aggregate was changed and the operations must
        ///     be retried.
        /// </exception>
        public void Put(TAggregateType aggregate, long originalVersion)
        {
            // No pending events? No put.
            if (aggregate.PendingEvents.Count == 0)
                return;

            lock (m_EventStorage)
            {
                // Enroll the aggregate stream
                if (!m_EventStorage.ContainsKey(aggregate.Key))
                {
                    // We should be working against revision 0 if this is our first Put()
                    if (originalVersion == 0)
                        m_EventStorage.Add(aggregate.Key, new List<IEvent>());
                    else
                        throw new ConcurrencyException(
                            string.Format(
                                "Cannot append events to '{0}' (Key: {1}). The aggregate does not exist, yet we're looking for version {2}..",
                                typeof (TAggregateType).FullName,
                                aggregate.Key.KeyString,
                                originalVersion));
                }

                // Get the existing event journal
                var eventJournal = m_EventStorage[aggregate.Key];
                if (eventJournal.Count != originalVersion)
                    throw new ConcurrencyException(
                        string.Format(
                            "Cannot append events to '{0}' (Key: {1}). Expected revision {2} but currently {3}",
                            typeof (TAggregateType).FullName,
                            aggregate.Key.KeyString,
                            originalVersion,
                            eventJournal.Count));

                // Append events
                foreach (var eventPending in aggregate.PendingEvents)
                    eventJournal.Add(eventPending);
            }

            // Propegate pending events
            foreach (var pendingEvent in aggregate.PendingEvents)
                EventWriter.WriteEvent(pendingEvent);
        }

        #endregion
    }
}