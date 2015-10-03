using System.Threading.Tasks;

namespace EventualConsistency.Framework
{
    /// <summary>
    ///     Handler for a specified type of command.
    /// </summary>
    public interface ICommandHandlerAsync<in TCommandType> : ICommandHandler
        where TCommandType : ICommand
    {
        /// <summary>
        ///     Handle the command asynchronously
        /// </summary>
        /// <param name="commandInstance">Command instance.</param>
        /// <returns>Awaitable task</returns>
        Task HandleAsync(TCommandType commandInstance);
    }
}