using System;
using System.Collections;
using NUnit.Framework;

namespace EventualConsistency.Framework.Testing
{
	/// <summary>
	/// 	Fluent sequence assertions
	/// </summary>
	public static class FluentSequenceExtensions
	{
		#region Static Methods

		/// <summary>
		/// 	A collection should start with the specified element
		/// </summary>
		/// <returns>The continuing enumerator of the sequence.</returns>
		/// <param name="sequence">Input sequence.</param>
		/// <param name="conditional">Conditional validation.</param>
		/// <typeparam name="TSubType">The type of element to expect.</typeparam>
		public static IEnumerator StartsWith<TSubType>(this IEnumerable sequence)
			where TSubType : class
		{
			IEnumerator enumerator = sequence.GetEnumerator ();
			if (!enumerator.MoveNext ())
				Assert.Fail ("The sequence was empty, so cannot satisfy StartsWith for the specified type'{0}'", typeof(TSubType).FullName);
			if (!(enumerator.Current is TSubType))
				Assert.Fail ("The sequence did not start with the expected '{0}', instead we have '{1}'", typeof(TSubType).FullName, enumerator.Current.GetType ().FullName);

			return enumerator;
		}

		/// <summary>
		/// 	A collection should start with the specified element, which satisfies a predicate.
		/// </summary>
		/// <returns>The continuing enumerator of the sequence.</returns>
		/// <param name="sequence">Input sequence.</param>
		/// <param name="conditional">Conditional validation.</param>
		/// <typeparam name="TSubType">The type of element to expect.</typeparam>
		public static IEnumerator StartsWith<TSubType>(this IEnumerable sequence, Predicate<TSubType> conditional)
			where TSubType : class
		{
			IEnumerator enumerator = sequence.GetEnumerator ();
			if (!enumerator.MoveNext ())
				Assert.Fail ("The sequence was empty, so cannot satisfy StartsWith for the specified type'{0}'", typeof(TSubType).FullName);
			if (!(enumerator.Current is TSubType))
				Assert.Fail ("The sequence did not start with the expected '{0}', instead we have '{1}'", typeof(TSubType).FullName, enumerator.Current.GetType ().FullName);
			if (!conditional (enumerator.Current as TSubType))
				Assert.Fail ("The '{0}' did not meet the expected condition.", enumerator.Current.GetType ().FullName);

			return enumerator;
		}

		/// <summary>
		/// 	A collection should continue with the specified element
		/// </summary>
		/// <returns>The continuing enumerator of the sequence.</returns>
		/// <param name="enumerator">Input enumerator.</param>
		/// <typeparam name="TSubType">The type of element to expect.</typeparam>
		public static IEnumerator AndThen<TSubType>(this IEnumerator enumerator)
			where TSubType : class
		{
			if (!enumerator.MoveNext ())
				Assert.Fail ("The sequence was empty, so cannot satisfy AndThen for the specified type '{0}'", typeof(TSubType).FullName);
			if (!(enumerator.Current is TSubType))
				Assert.Fail ("The sequence did not start with the expected '{0}', instead we have '{1}'", typeof(TSubType).FullName, enumerator.Current.GetType ().FullName);

			return enumerator;
		}

		/// <summary>
		/// 	A collection should continue with the specified element, which satisfies a predicate.
		/// </summary>
		/// <returns>The continuing enumerator of the sequence.</returns>
		/// <param name="enumerator">Input enumerator.</param>
		/// <param name="conditional">Conditional validation.</param>
		/// <typeparam name="TSubType">The type of element to expect.</typeparam>
		public static IEnumerator AndThen<TSubType>(this IEnumerator enumerator, Predicate<TSubType> conditional)
			where TSubType : class
		{
			if (!enumerator.MoveNext ())
				Assert.Fail ("The sequence was empty, so cannot satisfy AndThen for the specified type '{0}'", typeof(TSubType).FullName);
			if (!(enumerator.Current is TSubType))
				Assert.Fail ("The sequence did not start with the expected '{0}', instead we have '{1}'", typeof(TSubType).FullName, enumerator.Current.GetType ().FullName);
			if (!conditional (enumerator.Current as TSubType))
				Assert.Fail ("The '{0}' did not meet the expected condition.", enumerator.Current.GetType ().FullName);

			return enumerator;
		}

		/// <summary>
		/// 	The sequence should have no further values to reason about.
		/// </summary>
		/// <returns>The end.</returns>
		/// <param name="sequence">Sequence.</param>
		public static void AndEnd(this IEnumerator sequence)
		{
			if (sequence.MoveNext ())
				Assert.Fail ("The sequence did not end as expected. Current element is '{0}'", sequence.Current.GetType ().FullName);
		}

		#endregion
	}
}

