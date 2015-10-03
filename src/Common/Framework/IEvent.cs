namespace EventualConsistency.Framework
{
    /// <summary>
    ///     The IEvent interface defines the common behaviors of all events on aggregates.
    ///     Events represent specific outcomes on a domain object that explicitly occured
    ///     and are not phrased in the negative (for example AccountOpened, AccountOpenFailed
    ///     and not AccountOpen, AccountNotOpened)
    /// </summary>
    public interface IEvent
    {
    }
}