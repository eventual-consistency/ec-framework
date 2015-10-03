using System;

namespace EventualConsistency.Framework.Infrastructure
{
    /// <summary>
    ///     Base type for commands that implements common behaviors.
    /// </summary>
    public abstract class CommandBase : ICommand
    {
        /// <summary>
        ///     Initialize a new instance of CommandBase
        /// </summary>
        protected CommandBase()
        {
            // Allocate a new guid (if we deserialize, this will
            // be overwritten with the original).
            CommandId = Guid.NewGuid();
        }

        /// <summary>
        ///     Unique command Id
        /// </summary>
        /// <value>The command identifier.</value>
        public Guid CommandId { get; set; }
    }
}