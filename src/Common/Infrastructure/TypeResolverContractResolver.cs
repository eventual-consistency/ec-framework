using System;
using Newtonsoft.Json.Serialization;

namespace EventualConsistency.Framework.Infrastructure
{
    /// <summary>
    ///     Adapter for ITypeResolver that works with  IContractResolver from JSON.NET
    /// </summary>
    internal class TypeResolverContractResolver : DefaultContractResolver
    {
        #region Constructor(s)

        /// <summary>
        ///     Initialize a new TypeResolverContractResolver
        /// </summary>
        /// <param name="resolver">ITypeResolver to proxy</param>
        public TypeResolverContractResolver(ITypeResolver resolver)
        {
            // Validate arguments
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            // Build instance
            TypeResolver = resolver;
            
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Type Resolver
        /// </summary>
        protected ITypeResolver TypeResolver { get; }

        #endregion

        #region Methods

        /// <summary>
        ///     Resolve the contract type
        /// </summary>
        /// <param name="type">Type to resolve</param>
        /// <returns>JsonContract</returns>
        /// <remarks>
        ///     Overrides the default type constructor with the IoC provider if available.
        /// </remarks>
        public override JsonContract ResolveContract(Type type)
        {
            var contract =  base.ResolveContract(type);
            if (TypeResolver.CanResolve(type))
                contract.DefaultCreator = () => TypeResolver.ResolveSingleType(type);

            return contract;
        }

        #endregion
    }
}