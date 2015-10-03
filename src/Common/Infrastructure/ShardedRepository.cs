using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventualConsistency.Framework.Infrastructure
{
    /// <summary>
    ///     The ShardedRepository allows us to split the logical work of a repository
    ///     across N hosts. This assumes each shard is self-resilient to failure or
    ///     suitably highly available.
    /// </summary>
    public class ShardedRepository<TAggregateType, TKeyType> : IRepository<TAggregateType, TKeyType>
        where TAggregateType : IKeyedAggregate<TKeyType>, new()
        where TKeyType : IAggregateIdentity
    {
        /// <summary>
        ///     Initialize the sharded repository.
        /// </summary>
        /// <param name="shards">Shards to distribute data across</param>
        public ShardedRepository(IEnumerable<IRepository<TAggregateType, TKeyType>> shards)
        {
            Shards = shards.ToList();
        }

        /// <summary>
        ///     Shards
        /// </summary>
        protected IList<IRepository<TAggregateType, TKeyType>> Shards { get; set; }

        /// <summary>
        ///     Get an aggregate from the repository
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>Aggregate instance or null</returns>
        public TAggregateType Get(TKeyType key)
        {
            var shardIndex = key.GetPartitionAssignment(Shards.Count);
            return Shards[shardIndex].Get(key);
        }

        /// <summary>
        ///     Get an aggregate from the repository
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>Aggregate instance or null</returns>
        public async Task<TAggregateType> GetAsync(TKeyType key)
        {
            var shardIndex = key.GetPartitionAssignment(Shards.Count);
            return await Shards[shardIndex].GetAsync(key);
        }

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
        public void Put(TAggregateType aggregate, long originalVersion)
        {
            var shardIndex = aggregate.Key.GetPartitionAssignment(Shards.Count);
            Shards[shardIndex].Put(aggregate, originalVersion);
        }

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
        public async Task PutAsync(TAggregateType aggregate, long originalVersion)
        {
            var shardIndex = aggregate.Key.GetPartitionAssignment(Shards.Count);
            await Shards[shardIndex].PutAsync(aggregate, originalVersion);
        }
    }
}