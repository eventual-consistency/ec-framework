using System;
using System.Linq;
using EventualConsistency.Framework;
using EventualConsistency.Framework.Infrastructure;
using Microsoft.ServiceBus.Messaging;
using Polygamy;

namespace EventualConsistency.Providers.Azure
{
    /// <summary>
    ///     Event Bus reader that is backed by a Windows Azure Service Bus
    ///     topic subscription.
    /// </summary>
    public class AzureTopicEventBusReader : EventBusReaderBase
    {
        /// <summary>
        ///     Initialize a new instance of the AzureTopicEventBusReader
        /// </summary>
        /// <param name="sbsConnectionString">Service Bus Connection String</param>
        /// <param name="topicName">Topic Name</param>
        /// <param name="subscriptionName">Subscription Name</param>
        /// <param name="context">Bounded context</param>
        /// <param name="typeResolver">Type resolver for mapping JSON back to contract objects</param>
        /// <param name="dispatcher">Event Dispatcher for finding handlers for events after processing</param>
        public AzureTopicEventBusReader(string sbsConnectionString, string topicName, string subscriptionName,
            IBoundedContext context, ITypeResolver typeResolver, IDispatcher<IEvent> dispatcher) : base(context, typeResolver, dispatcher)
        {
            // Build instance
            CurrentSubscription = SubscriptionClient.CreateFromConnectionString(sbsConnectionString, topicName,
                subscriptionName);
        }
        
        /// <summary>
        ///     Subscription Client
        /// </summary>
        protected SubscriptionClient CurrentSubscription { get; }
        
        /// <summary>
        ///     Start listening to the message bus
        /// </summary>
        public override void Start()
        {
            var messageOptions = new OnMessageOptions();
            messageOptions.AutoComplete = false;
            messageOptions.ExceptionReceived += ProcessExceptionRecieved;
            CurrentSubscription.OnMessage(ProcessRecievedMessage, messageOptions);
        }

        /// <summary>
        ///     Stop listening
        /// </summary>
        public override void Stop()
        {
            if (!CurrentSubscription.IsClosed)
                CurrentSubscription.Close();
        }

        /// <summary>
        ///     Process a recieved exception from the SubscriptionClient
        /// </summary>
        /// <param name="sender">Sending Object</param>
        /// <param name="e">Event Arguments</param>
        protected virtual void ProcessExceptionRecieved(object sender, ExceptionReceivedEventArgs e)
        {
            // TODO: Audit exceptions here.
        }

        /// <summary>
        ///     Process the recieved messsage
        /// </summary>
        /// <param name="message">Message from the subscription</param>
        protected virtual void ProcessRecievedMessage(BrokeredMessage message)
        {
            if (message == null)
                return;
            
            // Read the body of the message
            var busItem = message.GetBody<EventBusMessage>();
            HandleEventBusMessage(busItem);
            message.Complete();
        }
    }
}