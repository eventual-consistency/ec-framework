using EventualConsistency.Framework;
using EventualConsistency.Framework.Infrastructure;
using Polygamy;
using EasyNetQ;
using System;
using System.Text;

namespace EventualConsistency.Providers.RabbitMq
{
    /// <summary>
    ///     RabbitMqEventBusReader listens to a RabbitMQ and dispatches messages. 
    /// </summary>
    /// <remarks>
    ///     It is assumed that any message that comes to this type can be handled, and the absence of a handler
    ///     means the message can be discarded from the queue. To handle situations where a single message must
    ///     be sent to multiple recipients that don't run in the same process, use RabbitMQ queue routing to
    ///     post multiple instances of the messages in a fan-out fashion.
    /// </remarks>
    public class RabbitMqEventBusReader : EventBusReaderBase, IDisposable
    {
        #region Constructor(s)

        /// <summary>
        ///     Initialize a new RabbitMqEventBusReader
        /// </summary>
        /// <param name="connectionSettings">Subscription Settings</param>
        /// <param name="context">Bounded context</param>
        /// <param name="resolver">Type resolver</param>
        /// <param name="dispatcher">Event dispatcher</param>
        public RabbitMqEventBusReader(RabbitMqSubscribeSettings connectionSettings, IBoundedContext context, ITypeResolver resolver, IDispatcher<IEvent> dispatcher) : base(context, resolver, dispatcher)
        {
            // Validate inputs
            if (connectionSettings == null)
                throw new ArgumentNullException(nameof(connectionSettings));

            // Build instance
            ConnectionSettings = connectionSettings;
        }

        #endregion

        #region IEventBus Implementation

        /// <summary>
        ///     Start listening
        /// </summary>
        public override void Start()
        {
            // Kill existing bus first
            if (Bus != null)
                Stop();

            Bus = RabbitHutch.CreateBus(ConnectionSettings.ConnectionString);

            var advanced = Bus.Advanced;
            var queue = advanced.QueueDeclare(ConnectionSettings.QueueName);
            advanced.Consume(queue, registrations => registrations.Add<EventBusMessage>((message, info) => HandleEventBusMessage(message.Body)));            
        }

        /// <summary>
        ///     Stop listening
        /// </summary>
        public override void Stop()
        {
            try
            {
                // Dispose bus
                if (Bus != null)
                    Bus.Dispose();
            }
            finally
            {
                Bus = null;
            }
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        ///     Dispose instance
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        ///     Dispose 
        /// </summary>
        /// <param name="managed">Managed resources only?</param>
        protected virtual void Dispose(bool managed)
        {
            Stop();

            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Dispose the instance if requiring finalization.
        /// </summary>
        ~RabbitMqEventBusReader()
        {
            Dispose(false);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Connection Settings
        /// </summary>
        protected RabbitMqSubscribeSettings ConnectionSettings { get; }

        /// <summary>
        ///     Connection
        /// </summary>
        protected IBus Bus { get; private set; }
        
        #endregion
    }
}
