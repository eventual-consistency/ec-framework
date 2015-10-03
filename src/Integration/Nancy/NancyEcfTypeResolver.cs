using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EventualConsistency.Framework;
using Nancy.TinyIoc;

namespace EventualConsistency.Integration.Nancy
{
    /// <summary>
    ///     The NancyEcfTypeResolver is an EventualConsistency ITypeResolver variant
    ///     that provides resolution of types using Nancy's built in TinyIoC implementation.
    /// </summary>
    public class NancyEcfTypeResolver : ITypeResolver
    {
        #region Private Fields

        /// <summary>
        ///     Nancy Container
        /// </summary>
        private readonly TinyIoCContainer _nancyContainer;

        #endregion

        #region Constructor(s)

        /// <summary>
        ///     Initialize a new NancyEcfTypeResolver
        /// </summary>
        /// <param name="container">TinyIoCContainer instance</param>
        public NancyEcfTypeResolver(TinyIoCContainer container)
        {
            // Validate Arguments
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            // Build instance
            _nancyContainer = container;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Can we resolve the specified type?
        /// </summary>
        /// <typeparam name="TResultType">Type to resolve</typeparam>
        /// <returns>True if IoC can resolve, false otherwise</returns>
        public bool CanResolve<TResultType>()
            where TResultType : class
        {
            return _nancyContainer.CanResolve<TResultType>();
        }

        /// <summary>
        ///     Can we resolve the specified type?
        /// </summary>
        /// <param name="resultType">Type to resolve</param>
        /// <returns>True if IoC can resolve, false otherwise</returns>
        public bool CanResolve(Type resultType)
        {
            // Validate Arguments
            if (resultType == null)
                throw new ArgumentNullException(nameof(resultType));

            return _nancyContainer.CanResolve(resultType);
        }

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
        public TResultType ResolveSingleType<TResultType>()
            where TResultType : class
        {
            var result = _nancyContainer.Resolve<TResultType>();
            if (result == null)
                throw new NancyEcfTypeResolutionException(
                    String.Format(CultureInfo.CurrentUICulture, NancyMessages.CouldNotFindInstancesOfType,
                        typeof (TResultType).FullName));
            return result;
        }

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
        public object ResolveSingleType(Type typeToResolve)
        {
            // Validate arguments
            if (typeToResolve == null)
                throw new ArgumentNullException(nameof(typeToResolve));

            // Check IoC
            var result = _nancyContainer.Resolve(typeToResolve);
            if (result == null)
                throw new NancyEcfTypeResolutionException(
                    String.Format(CultureInfo.CurrentUICulture, NancyMessages.CouldNotFindInstancesOfType,
                        typeToResolve.FullName));
            return result;

        }

        /// <summary>
        ///     Obtain all registered implementations
        /// </summary>
        /// <typeparam name="TResultType">Result Type</typeparam>
        /// <returns>Instance of each registered result type</returns>
        public IEnumerable<TResultType> ResolveMultiple<TResultType>()
            where TResultType : class
        {
            var searchType = typeof (TResultType);

            var result = _nancyContainer.ResolveAll<TResultType>()?.ToList();
            if (result == null || !result.Any())
                throw new NancyEcfTypeResolutionException(
                    string.Format(CultureInfo.CurrentUICulture, NancyMessages.CouldNotFindInstancesOfType,
                        searchType.FullName));

            return result;
        }

        /// <summary>
        ///     Obtain all registered implementations and return as a dynamic
        /// </summary>
        /// <param name="searchType">Search Type</param>
        /// <returns>Instance of each registered result type</returns>
        public IEnumerable<dynamic> ResolveMultipleDynamic(Type searchType)
        {
            // Validate arguments
            if (searchType == null)
                throw new ArgumentNullException(nameof(searchType));

            var result = _nancyContainer.ResolveAll(searchType)?.ToList();
            if (result == null || !result.Any())
                throw new NancyEcfTypeResolutionException(
                    string.Format(CultureInfo.CurrentUICulture, NancyMessages.CouldNotFindInstancesOfType,
                        searchType.FullName));

            return result;
        }

        #endregion
    }
}