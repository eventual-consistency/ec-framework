using System.Threading.Tasks;

namespace EventualConsistency.Framework
{
    /// <summary>
    ///     The ICommandBusAsync defines the behaviours of an asynchronous command
    ///     bus for EventualConsistency systems.
    /// </summary>
    public interface ICommandBusAsync
    {
        /// <summary>
        ///     Send the specified command to the domain.
        /// </summary>
        /// <typeparam name="TCommandType">Type of command</typeparam>
        /// <param name="command">Command to send</param>
        /// <returns>Resultant task</returns>
        Task SendAsync<TCommandType>(TCommandType command)
            where TCommandType : ICommand;
    }
}