using System;
using System.Collections.Generic;

namespace EventualConsistency.Framework
{
    /// <summary>
    ///     The ITypeResolver is a simple contract for EventualConsistency that allows us to be
    ///     indepdant of the underlying IoC container. Type resolvers can implement support for
    ///     various IoC systems (Tiny, Autofac, Ninject etc) as required.
    /// </summary>
    public interface ITypeResolver
    {
        /// <summary>
        ///     Can we resolve the specified type?
        /// </summary>
        /// <typeparam name="TResultType">Type to resolve</typeparam>
        /// <returns>True if IoC can resolve, false otherwise</returns>
        bool CanResolve<TResultType>()
            where TResultType : class;

        /// <summary>
        ///     Can we resolve the specified type?
        /// </summary>
        /// <param name="resultType">Type to resolve</param>
        /// <returns>True if IoC can resolve, false otherwise</returns>
        bool CanResolve(Type resultType);

        /// <summary>
        ///     Obtain a single instance/implementation of a type.
        /// </summary>
        /// <typeparam name="TResultType">Result Type</typeparam>
        /// <returns>Instance of result type</returns>
        /// <remarks>
        ///     ECF generally assumes that all types are self-disposing, or that the parent
        ///     container that activated them is managing types. Registering multiple types
        ///     with the same prefix should cause ResolveSingleType to fail, as there is no
        ///     canonical down-mapping to decide the winner.
        /// </remarks>
        TResultType ResolveSingleType<TResultType>()
            where TResultType : class;

        /// <summary>
        ///     Obtain a single instance/implementation of a type.
        /// </summary>
        /// <param name="typeToResolve">Type to reoslver</param>>
        /// <returns>Instance of result type</returns>
        /// <remarks>
        ///     ECF generally assumes that all types are self-disposing, or that the parent
        ///     container that activated them is managing types. Registering multiple types
        ///     with the same prefix should cause ResolveSingleType to fail, as there is no
        ///     canonical down-mapping to decide the winner.
        /// </remarks>
        object ResolveSingleType(Type typeToResolve);


        /// <summary>
        ///     Obtain all registered implementations
        /// </summary>
        /// <typeparam name="TResultType">Result Type</typeparam>
        /// <returns>Instance of each registered result type</returns>
        IEnumerable<TResultType> ResolveMultiple<TResultType>()
            where TResultType : class;

        /// <summary>
        ///     Obtain all registered implementations and return as a dynamic
        /// </summary>
        /// <param name="searchType">Search Type</param>
        /// <returns>Instance of each registered result type</returns>
        IEnumerable<dynamic> ResolveMultipleDynamic(Type searchType);
    }
}