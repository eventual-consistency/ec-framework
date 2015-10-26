using System;
using EventualConsistency.Framework;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EventualConsistency.Framework.Testing
{
	/// <summary>
	/// 	The TestEventBus allows for testing of a subject domain without sending real
	/// 	events to a queue externally.
	/// </summary>
	public class TestEventBus : IEventBusWriter
	{
		#region Constructor(s)

		/// <summary>
		/// 	Initializes a new instance of the <see cref="CorePrime.Domain.Tests.TestEventBus"/> class.
		/// </summary>
		public TestEventBus ()
		{
			EventStreamData = new List<IEvent> ();
		}

		#endregion

		#region Properties

		/// <summary>
		/// 	Event stream
		/// </summary>
		/// <value>The event stream data.</value>
		protected IList<IEvent> EventStreamData { get; }

		/// <summary>
		/// 	Gets the events.
		/// </summary>
		/// <value>The events.</value>
		public IReadOnlyCollection<IEvent> Events { get { return new ReadOnlyCollection<IEvent>(EventStreamData); } }

		#endregion

		#region IEventBusWriter implementation

		/// <summary>
		/// 	Write event
		/// </summary>
		/// <param name="eventInstance">Event instance.</param>
		public void WriteEvent (IEvent eventInstance)
		{
			EventStreamData.Add (eventInstance);
		}

		#pragma warning disable 1998

		/// <summary>
		/// 	Write event asynchronously.
		/// </summary>
		/// <returns>Task continuation.</returns>
		/// <param name="eventInstance">Event instance.</param>
		public async Task WriteEventAsync (IEvent eventInstance)
		{
			WriteEvent (eventInstance);
		}

		#pragma warning restore 1998

		#endregion
	}
}

