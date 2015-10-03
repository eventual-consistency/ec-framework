namespace EventualConsistency.Providers.Azure
{
    /// <summary>
    ///     Framework constants specific to the Azure related classes.
    /// </summary>
    public static class AzureFrameworkConstants
    {
        /// <summary>
        ///     Message slot for the name of the bounded context
        /// </summary>
        /// <remarks>
        ///     Used to ensure that we're filtering appropriately in
        ///     messaged message streams on a shared bus.
        /// </remarks>
        public const string ContextMessageProperty = "BoundedContext";

        /// <summary>
        ///     Short event type name
        /// </summary>
        public const string EventTypeMessageProperty = "EventTypeName";
    }
}