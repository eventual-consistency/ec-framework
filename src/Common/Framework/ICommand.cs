using System;

namespace EventualConsistency.Framework
{
    /// <summary>
    ///     Commands for publishing to the domain. Commands are imperative 'doing' sequences that
    ///     represent distinct requests from users or software components to effect a change.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        ///     Unique command Id
        /// </summary>
        /// <value>The command identifier.</value>
        Guid CommandId { get; set; }
    }
}