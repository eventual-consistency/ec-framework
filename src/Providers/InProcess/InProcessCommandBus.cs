using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventualConsistency.Framework;

namespace EventualConsistency.Providers.InProcess
{
    /// <summary>
    ///     The InProcessCommandBus exeuctes command-handlers for a domain
    ///     in the currently executing process.
    /// </summary>
    public class InProcessCommandBus : ICommandBusAsync
    {
        #region Constructor(s)

        /// <summary>
        ///     Initialize a new instance of the InProcessCommandBus with a set of
        ///     defined command handlers
        /// </summary>
        /// <param name="commandHandlers">Command Handlers</param>
        public InProcessCommandBus(IEnumerable<ICommandHandler> commandHandlers)
        {
            CommandHandlers = commandHandlers;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Command Handlers
        /// </summary>
        protected IEnumerable<ICommandHandler> CommandHandlers { get; }

        #endregion

        #region ICommandBusAsync Implementation

        /// <summary>
        ///     Send the specified command to the domain.
        /// </summary>
        /// <typeparam name="TCommandType">Type of command</typeparam>
        /// <param name="command">Command to send</param>
        /// <returns>Resultant task</returns>
        public async Task SendAsync<TCommandType>(TCommandType command) where TCommandType : ICommand
        {
            // Validate Arguments
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var found = false;

            // Apply each supported handler
            foreach (var handler in CommandHandlers.Where(_ => _.CanHandle(command)))
            {
                // Is this an async handler?
                var asyncHandler = handler as ICommandHandlerAsync<TCommandType>;
                var syncHandler = handler as ICommandHandler<TCommandType>;
                if (asyncHandler != null)
                {
                    found = true;
                    await asyncHandler.HandleAsync(command);
                }
                else if (syncHandler != null)
                {
                    found = true;
                    syncHandler.Handle(command);
                }
            }

            // Nobody picked up the phone, so let the caller know.
            if (!found)
                throw new UnknownCommandException(command.GetType().FullName);
        }

        #endregion
    }
}