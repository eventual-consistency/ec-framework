using EventualConsistency.Framework;
using EasyNetQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventualConsistency.Framework.Infrastructure;
using EasyNetQ.Topology;

namespace EventualConsistency.Providers.RabbitMq
{
    /// <summary>
    ///     The RabbitMqEventBusWriter writes events to a nominated RabbitMQ endpoint/queue.
    /// </summary>
    public class RabbitMqEventBusWriter : IEventBusWriter, IDisposable
    {
        #region Constructor(s)

        /// <summary>
        ///     Initialize a new instance of RabbitMqEventBusWriter
        /// </summary>
        public RabbitMqEventBusWriter(RabbitMqPublishSettings connectionSettings)
        {
            // Validate inputs
            if (connectionSettings == null)
                throw new ArgumentNullException(nameof(connectionSettings));

            ConnectionSettings = connectionSettings;
            Bus = RabbitHutch.CreateBus(ConnectionSettings.ConnectionString);

            var advanced = Bus.Advanced;
            Exchange = advanced.ExchangeDeclare(connectionSettings.ExchangeName, ExchangeType.Topic);
        }

        #endregion

        #region IEventBusWriter Implementation

        /// <summary>
        ///     Write an event to the RabbitMQ instance.
        /// </summary>
        /// <param name="eventInstance">Event instance</param>
        public void WriteEvent(IEvent eventInstance)
        {
            // No event, no send.
            if (eventInstance == null)
                return;

            var message = new EventBusMessage(eventInstance);

            Bus.Advanced.Publish(Exchange, String.Empty, false, false, new Message<EventBusMessage>(message));
        }

        /// <summary>
        ///     Write an event asynchronously.
        /// </summary>
        /// <param name="eventInstance">Event Instance</param>
        /// <returns></returns>
        public async Task WriteEventAsync(IEvent eventInstance)
        {
            // No event, no send.
            if (eventInstance == null)
                return;

            var message = new EventBusMessage(eventInstance);
            await Bus.Advanced.PublishAsync(Exchange, String.Empty, false, false, new Message<EventBusMessage>(message));
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
            if (Bus != null)
            {
                Bus.Dispose();
                Bus = null;
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Dispose the instance if requiring finalization.
        /// </summary>
        ~RabbitMqEventBusWriter()
        {
            Dispose(false);
        }

        #endregion


        #region Properties

        /// <summary>
        ///     Bus Instance
        /// </summary>
        private IBus Bus { get; set; }

        /// <summary>
        ///     Exchange
        /// </summary>
        private IExchange Exchange { get; set;  }

        /// <summary>
        ///     Connection settings
        /// </summary>
        private RabbitMqPublishSettings ConnectionSettings { get; }

        #endregion
    }
}
