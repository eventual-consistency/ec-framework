using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace EventualConsistency.Framework.Testing
{
    /// <summary>
    ///     Extension methods for Moq Mock
    /// </summary>
    public static class MockExtensions
    {
        /// <summary>
        ///     Check that events occured in expected sequence.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="mock">Mock object</param>
        /// <param name="expressions">Expressions to check</param>
        public static void ExpectsInOrder<T>(this Mock<T> mock, params Expression<Action<T>>[] expressions)
            where T : class
        {
            // All closures have the same instance of sharedCallCount
            var sharedCallCount = 0;
            for (var i = 0; i < expressions.Length; i++)
            {
                // Each closure has it's own instance of expectedCallCount
                var expectedCallCount = i;
                mock.Setup(expressions[i]).Callback(
                    () =>
                    {
                        Assert.AreEqual(expectedCallCount, sharedCallCount);
                        sharedCallCount++;
                    });
            }
        }
    }
}