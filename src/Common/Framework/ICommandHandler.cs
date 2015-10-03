namespace EventualConsistency.Framework
{
    /// <summary>
    ///     Non-Generic CommandHandler interface
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        ///     Can this handler process the specified command?
        /// </summary>
        /// <param name="commandInstance">Command instance</param>
        /// <returns>True if supported, false otherwise</returns>
        bool CanHandle(ICommand commandInstance);
    }

    /// <summary>
    ///     Handler for a specified type of command.
    /// </summary>
    public interface ICommandHandler<TCommandType> : ICommandHandler
        where TCommandType : ICommand
    {
        /// <summary>
        ///     Handle an instance of the specified command type.
        /// </summary>
        /// <param name="commandInstance">Command instance.</param>
        void Handle(TCommandType commandInstance);
    }
}