using System.Threading.Tasks;
using EventualConsistency.Framework;
using Microsoft.ServiceBus.Messaging;
using EventualConsistency.Framework.Infrastructure;

namespace EventualConsistency.Providers.Azure
{
    /// <summary>
    ///     Azure Topic Event Bus Writer - Distributes events to an Azure ServiceBus topic.
    /// </summary>
    public class AzureTopicEventBusWriter : IEventBusWriter
    {
        /// <summary>
        ///     Initialize a new instance of the AzureEventBusWriter
        /// </summary>
        /// <param name="context">Bounded Context</param>
        /// <param name="sbsConnectionString">Service Bus Connection String</param>
        /// <param name="topicName">Topic Name</param>
        public AzureTopicEventBusWriter(IBoundedContext context, string sbsConnectionString, string topicName)
        {
            Context = context;
            TopicWriter = TopicClient.CreateFromConnectionString(sbsConnectionString, topicName);
        }

        /// <summary>
        ///     Topic Client for this instance
        /// </summary>
        protected TopicClient TopicWriter { get; }

        /// <summary>
        ///     Bounded Context
        /// </summary>
        protected IBoundedContext Context { get; }

        /// <summary>
        ///     Write an event to the topic
        /// </summary>
        /// <param name="eventInstance">Event Instance</param>
        public void WriteEvent(IEvent eventInstance)
        {
            var message = CreateTransportItem(eventInstance);
            TopicWriter.Send(message);
        }

        /// <summary>
        ///     Write an event to the topic
        /// </summary>
        /// <param name="eventInstance">Event Instance</param>
        public async Task WriteEventAsync(IEvent eventInstance)
        {
            var message = CreateTransportItem(eventInstance);
            await TopicWriter.SendAsync(message);
        }

        /// <summary>
        ///     Create the transport item wrapper for this instance of an event
        /// </summary>
        /// <param name="eventInstance">Event Instance</param>
        /// <returns>Transport item</returns>
        protected BrokeredMessage CreateTransportItem(IEvent eventInstance)
        {
            var item = new EventBusMessage(eventInstance);

            var message = new BrokeredMessage(item);
            message.Properties.Add(AzureFrameworkConstants.ContextMessageProperty, Context.ContextName);
            message.Properties.Add(AzureFrameworkConstants.EventTypeMessageProperty, item.EventType);

            return message;
        }
    }
}