using System;

namespace EventualConsistency.Providers.RabbitMq
{
    /// <summary>
    ///     RabbitMq connection settings
    /// </summary>
    [Serializable]
    public class RabbitMqSubscribeSettings
    {
        /// <summary>
        ///     Connection string
        /// </summary>
        public string ConnectionString { get; set; }
        
        /// <summary>
        ///     Queue Name
        /// </summary>
        public string QueueName { get; set;  }

        /// <summary>
        ///     Subscription Id
        /// </summary>
        public string SubscriptionId { get; set;  }
    }
}
