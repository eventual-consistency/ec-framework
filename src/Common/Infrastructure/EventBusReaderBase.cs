using Polygamy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventualConsistency.Framework.Infrastructure
{
    /// <summary>
    ///     Common base class for event bus readers. 
    /// </summary>
    public abstract class EventBusReaderBase : IEventBusReader
    {
        /// <summary>
        ///     Initialize a new instance of the EventBusReaderBase
        /// </summary>
        /// <param name="context">Bounded context</param>
        /// <param name="typeResolver">Type resolver for mapping JSON back to contract objects</param>
        /// <param name="dispatcher">Event Dispatcher for finding handlers for events after processing</param>
        protected EventBusReaderBase(IBoundedContext context, ITypeResolver typeResolver, IDispatcher<IEvent> dispatcher)
        {
            // Validate Arguments
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (typeResolver == null)
                throw new ArgumentNullException(nameof(typeResolver));
            if (dispatcher == null)
                throw new ArgumentNullException(nameof(dispatcher));

            // Build instance
            BoundedContext = context;
            Dispatcher = dispatcher;
            TypeResolver = typeResolver;
        }

        /// <summary>
        ///     Bounded Context
        /// </summary>
        protected IBoundedContext BoundedContext { get; }
        
        /// <summary>
        ///     Type Resolver
        /// </summary>
        protected IDispatcher<IEvent> Dispatcher { get; }

        /// <summary>
        ///     Type Resolver
        /// </summary>
        protected ITypeResolver TypeResolver { get; }

        /// <summary>
        ///     Start listening to the message bus
        /// </summary>
        public abstract void Start();

        /// <summary>
        ///     Stop listening
        /// </summary>
        public abstract void Stop();
        
        /// <summary>
        ///     Process the recieved messsage
        /// </summary>
        /// <param name="message">Message from the subscription</param>
        protected void HandleEventBusMessage(EventBusMessage message)
        {
            if (message == null)
                return;
            
            // Find the type - If not part of our bounded context, ignore.
            var targetType =
                BoundedContext.EventTypes.Where(x => string.Equals(x.Name, message.EventType)).FirstOrDefault();
            if (targetType == null)
                return;

            // Unpack JSON into event data
            var eventInstance = targetType.UnpackJsonEvent(message.EventJson, TypeResolver);

            // Execute event handlers
            Dispatcher.Dispatch(eventInstance);
        }
    }
}
