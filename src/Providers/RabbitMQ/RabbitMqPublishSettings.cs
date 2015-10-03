using System;

namespace EventualConsistency.Providers.RabbitMq
{
    /// <summary>
    ///     RabbitMq publish settings
    /// </summary>
    [Serializable]
    public class RabbitMqPublishSettings
    {
        /// <summary>
        ///     Connection string
        /// </summary>
        public string ConnectionString { get; set; }
        
        /// <summary>
        ///     Exchange name
        /// </summary>
        public string ExchangeName { get; set;  }
    }
}
