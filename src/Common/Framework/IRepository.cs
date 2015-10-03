using System.Threading.Tasks;

namespace EventualConsistency.Framework
{
    /// <summary>
    ///     The IRepository interface declares the behaviors of an aggregate repository that allows for key-value
    ///     lookups and storage of event streams.
    /// </summary>
    public interface IRepository<TAggregateType, TKeyType>
        where TAggregateType : IKeyedAggregate<TKeyType>
        where TKeyType : IAggregateIdentity
    {
        /// <summary>
        ///     Get an aggregate from the repository
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>Aggregate instance or null</returns>
        TAggregateType Get(TKeyType key);

        /// <summary>
        ///     Get an aggregate from the repository
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>Aggregate instance or null</returns>
        Task<TAggregateType> GetAsync(TKeyType key);

        /// <summary>
        ///     Store an aggregate into the repository.
        /// </summary>
        /// <param name="aggregate">Aggregate.</param>
        /// <param name="originalVersion">Original version number to change against.</param>
        /// <remarks>
        ///     Returning without an exception indicates success.
        /// </remarks>
        /// <exception cref="EventualConsistency.Framework.ConcurrencyException">
        ///     The aggregate was changed and the operations must
        ///     be retried.
        /// </exception>
        void Put(TAggregateType aggregate, long originalVersion);

        /// <summary>
        ///     Store an aggregate into the repository.
        /// </summary>
        /// <param name="aggregate">Aggregate.</param>
        /// <param name="originalVersion">Original version number to change against.</param>
        /// <remarks>
        ///     Returning without an exception indicates success.
        /// </remarks>
        /// <exception cref="EventualConsistency.Framework.ConcurrencyException">
        ///     The aggregate was changed and the operations must
        ///     be retried.
        /// </exception>
        Task PutAsync(TAggregateType aggregate, long originalVersion);
    }
}