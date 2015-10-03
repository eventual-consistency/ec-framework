namespace EventualConsistency.Framework
{
    /// <summary>
    ///     The ICommandBus describes the behaviors of the write-side of CQRS. This is
    ///     the entry point of external commands to the system.
    /// </summary>
    public interface ICommandBus
    {
        /// <summary>
        ///     Send a command on the command bus
        /// </summary>
        /// <remarks>
        ///     Returning without an exception is an implicit acknowledgement of
        ///     the command being persisted.
        /// </remarks>
        /// <param name="command">Command to send.</param>
        void Send(ICommand command);
    }
}