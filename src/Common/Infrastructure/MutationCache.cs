using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace EventualConsistency.Framework.Infrastructure
{
    /// <summary>
    ///     Mutation Cache for state transition functions. Allows rapid lookup and
    ///     execution of OnEvent(IEvent) methods on aggregate roots during replay.
    /// </summary>
    public static class MutationCache<TStateObject>
    {
        /// <summary>
        ///     Mutator dictionary (keyed by type)
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")] private static readonly
            Dictionary<Type, MethodInfo> mutators;

        /// <summary>
        ///     Build the mutation function cache
        /// </summary>
        static MutationCache()
        {
            // Build the mutators list for the specified type.
            mutators = typeof (TStateObject).GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                                                        BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                .Where(method => method.Name == "OnEvent" && method.GetParameters().Count() == 1)
                .ToDictionary(method => method.GetParameters().First().ParameterType, method => method);
        }

        /// <summary>
        ///     Apply an event to the state object
        /// </summary>
        /// <param name="state">State.</param>
        /// <param name="eventInstance">Event instance.</param>
        /// <typeparam name="TEventType">The 1st type parameter.</typeparam>
        public static void ApplyEvent<TEventType>(TStateObject state, TEventType eventInstance)
        {
            // Get the event type
            var processType = typeof (TEventType);

            // If the cache is invoked by a generic, resolve against the specific instance type.
            if (processType.IsAbstract || processType.IsInterface)
                processType = eventInstance.GetType();

            // No such mutator?
            if (!mutators.ContainsKey(processType))
                throw new Exception("No such OnEvent: " + processType.FullName);

            // Invoke the method
            mutators[processType].Invoke(state, new object[] {eventInstance});
        }
    }
}