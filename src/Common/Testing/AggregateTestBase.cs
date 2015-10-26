using System;
using EventualConsistency.Framework;
using Moq;
using NUnit.Framework;
using EventualConsistency.Providers.InProcess;

namespace EventualConsistency.Framework.Testing
{
	/// <summary>
	/// 	Aggregate test base class
	/// </summary>
	public abstract class AggregateTestBase<TAggregateType, TAggregateKeyType>
		where TAggregateType : class, IKeyedAggregate<TAggregateKeyType>
		where TAggregateKeyType : IAggregateIdentity
	{
		#region Constructor(s)

		/// <summary>
		/// 	Initializes a new instance of the <see cref="CorePrime.Domain.Tests.AggregateTestBase`2"/> class.
		/// </summary>
		protected AggregateTestBase()
		{

		}

		#endregion

		#region NUnit Setup and Teardown

		/// <summary>
		/// 	Performs per-test setup
		/// </summary>
		[SetUp]
		public void SetupTest()
		{
			EventBus = new TestEventBus ();
			Repository = new MemoryRepository<TAggregateType, TAggregateKeyType>(CreateBlankAggregate, EventBus);
		}

		#endregion

		#region Properties

		public IRepository<TAggregateType, TAggregateKeyType> Repository { get; set; }

		/// <summary>
		/// 	Event Bus
		/// </summary>
		/// <value>The event bus.</value>
		public TestEventBus EventBus { get; set; }

		#endregion

		#region Abstract Methods

		/// <summary>
		/// 	Creates the blank aggregate.
		/// </summary>
		/// <returns>The blank aggregate.</returns>
		public abstract TAggregateType CreateBlankAggregate ();

		#endregion

		#region Helper Methods

		/// <summary>
		/// 	Perform an action against a keyed aggregate.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="action">Action.</param>
		public void WithAggregate(TAggregateKeyType key, Action<TAggregateType> action)
		{
			var aggregate = Repository.Get(key);
			var originalRevision = aggregate.RevisionNumber;
			action (aggregate);
			Repository.Put (aggregate, originalRevision);
		}

		#endregion
	}
}

