namespace EventualConsistency.Framework
{
    /// <summary>
    ///     State Object for an aggregate. Can be collapsed onto the aggregate type itself,
    ///     but generally is good to seperate out the aggregate logic versus the event consequence
    ///     processing.
    /// </summary>
    public interface IStateObject
    {
    }
}