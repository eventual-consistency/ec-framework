using System;
using System.Threading.Tasks;

namespace EventualConsistency.Framework.Infrastructure
{
    /// <summary>
    ///     The CommandHandlerBase is a base type for command handlers that deal only
    ///     with a single aggregate root. This is most scenarios usually, as handlers that
    ///     deal with multiple aggregates should be broken up as sagas for maintainability.
    /// </summary>
    public abstract class CommandHandlerBase<TAggregateType, TAggregateKeyType>
        where TAggregateType : IKeyedAggregate<TAggregateKeyType>
        where TAggregateKeyType : IAggregateIdentity
    {
        /// <summary>
        ///     Initialize a new instance of the base class and configure instance properties.
        /// </summary>
        /// <param name="repository">Repository.</param>
        protected CommandHandlerBase(IRepository<TAggregateType, TAggregateKeyType> repository)
        {
            Repository = repository;
        }

        /// <summary>
        ///     Repository of aggregates that this handler deals with.
        /// </summary>
        /// <value>The repository for the relevent aggregates.</value>
        protected IRepository<TAggregateType, TAggregateKeyType> Repository { get; }

        /// <summary>
        ///     Perform an action against a single aggregate.
        /// </summary>
        /// <param name="correlationId">Correlation identifier.</param>
        /// <param name="aggregateKey">Aggregate key.</param>
        /// <param name="action">Action.</param>
        protected void WithAggregate(Guid correlationId, TAggregateKeyType aggregateKey, Action<TAggregateType> action)
        {
            // Fetch the aggregate
            var aggregate = Repository.Get(aggregateKey);
            var originalVersion = aggregate == null ? 0 : aggregate.RevisionNumber;

            // Apply the action
            action(aggregate);

            // Put the aggregate
            if (aggregate.PendingEvents.Count > 0)
                Repository.Put(aggregate, originalVersion);
        }

        /// <summary>
        ///     Perform an action against a single aggregate.
        /// </summary>
        /// <param name="correlationId">Correlation identifier.</param>
        /// <param name="aggregateKey">Aggregate key.</param>
        /// <param name="action">Action.</param>
        protected async Task WithAggregateAsync(Guid correlationId, TAggregateKeyType aggregateKey,
            Action<TAggregateType> action)
        {
            // Fetch the aggregate
            var aggregate = await Repository.GetAsync(aggregateKey);
            var originalVersion = aggregate == null ? 0 : aggregate.RevisionNumber;

            // Apply the action (model updates are always synchronous)
            action(aggregate);

            // Put the aggregate
            if (aggregate.PendingEvents.Count > 0)
                await Repository.PutAsync(aggregate, originalVersion);
        }
    }
}