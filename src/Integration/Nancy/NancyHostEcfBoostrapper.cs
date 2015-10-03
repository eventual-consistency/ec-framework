using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using EventualConsistency.Framework;
using EventualConsistency.Framework.Infrastructure;
using Nancy;
using Nancy.TinyIoc;

namespace EventualConsistency.Integration.Nancy
{
    /// <summary>
    ///     The NancyHostEcfBoostrapper performs some automatic configuration of an ECF application inside Nancy and promotes
    ///     the
    ///     super-duper-happy path as much as reasonably possible.
    /// </summary>
    /// <remarks>
    ///     The specific types are registered using the assemblies currently loaded and all dependent assemblies:
    ///     <list type="bullet">
    ///         <description>Any implementations of IAggregate</description>
    ///         <description>Any implementations of ICommandHandler.</description>
    ///         <description>Any implementations of IeventHandler.</description>
    ///         <description>
    ///             Any implementation of IBoundedContext (Fails on multiple, will register AutoAssemblyBoundedContext
    ///             if none)
    ///         </description>
    ///         <description>Any implementation of ICommandBus (Fails on multiple, will skip if none)</description>
    ///         <description>Any implementation of ICommandBusAsync (Fails on multiple, will skip if none)</description>
    ///         <description>The NancyEcfTypeResolver for ITypeResolver</description>
    ///     </list>
    ///     Each of these specific behaviours can be adjusted by derriving from the NancyHostEcfBoostrapper in your own project
    ///     and overriding
    ///     the various protected methods that perform the individual registrations.
    /// </remarks>
    public class NancyHostEcfBoostrapper : DefaultNancyBootstrapper
    {
        #region DefaultNancyBootstrapper Overrides

        /// <summary>
        ///     Configure application level container
        /// </summary>
        /// <param name="container">Container instance</param>
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            // Aggregates
            RegisterAggregateTypes(container);

            // ICommandHandler
            RegisterCommandHandlers(container);

            // IEventHandler
            RegisterEventHandlers(container);

            // IBoundedContext
            RegisterBoundedContexts(container);

            // ICommandBusSync
            RegisterCommandBusSync(container);

            // ICommandBusAsync
            RegisterCommandBusAsync(container);

            // ITypeResolver
            RegisterTypeResolver(container);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Get assemblies that we should look for types within.
        /// </summary>
        /// <returns>Candidate assemblies list</returns>
        protected virtual IEnumerable<Assembly> GetCandidateAssemblies()
        {
            var candidateList = AppDomain.CurrentDomain.GetAssemblies().ToList();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                LoadReferencesRecursive(candidateList, assembly);

            // Now return the retrieid version.
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        /// <summary>
        ///     Load assembly references recursive
        /// </summary>
        /// <param name="loadedAssemblies">Assemblies</param>
        /// <param name="assembly">Assembly to recurse</param>
        private static void LoadReferencesRecursive(List<Assembly> loadedAssemblies, Assembly assembly)
        {
            foreach (var namedAssembly in assembly.GetReferencedAssemblies())
            {
                var loaded = Assembly.Load(namedAssembly);
                if (!loadedAssemblies.Contains(loaded))
                {
                    loadedAssemblies.Add(loaded);
                    LoadReferencesRecursive(loadedAssemblies, loaded);
                }
            }
        }


        /// <summary>
        ///     Register all aggregates
        /// </summary>
        /// <param name="container">Container instance</param>
        protected virtual void RegisterAggregateTypes(TinyIoCContainer container)
        {
            var aggregateTypes = FindTypes<IAggregate>().ToList();
            if (aggregateTypes.Count == 0)
                return;

            // Register as IAggregate 
            container.RegisterMultiple<IAggregate>(aggregateTypes);

            // Register as self
            foreach (var typeToRegister in aggregateTypes)
                container.Register(typeToRegister);
        }

        /// <summary>
        ///     Register all command handlers
        /// </summary>
        /// <param name="container">Container instance</param>
        protected virtual void RegisterCommandHandlers(TinyIoCContainer container)
        {
            var commandHandlerTypes = FindTypes<ICommandHandler>().ToList();
            if (commandHandlerTypes.Count > 0)
                container.RegisterMultiple<ICommandHandler>(commandHandlerTypes);
        }

        /// <summary>
        ///     Register all event handlers
        /// </summary>
        /// <param name="container">Container instance</param>
        protected virtual void RegisterEventHandlers(TinyIoCContainer container)
        {
            var eventHandlerTypes = FindTypes<IEventHandler>().ToList();
            if (eventHandlerTypes.Count > 0)
                container.RegisterMultiple<IEventHandler>(eventHandlerTypes);
        }

        /// <summary>
        ///     Register all synchronous command buses
        /// </summary>
        /// <param name="container">Container instance</param>
        protected virtual void RegisterCommandBusSync(TinyIoCContainer container)
        {
            const string configHint = "ecf-hint:nancy-commandbus";

            var candidateList = FindTypes<ICommandBus>().ToList();
            if (candidateList.Count > 1)
                RaiseAmbiguousTypeException<ICommandBus>(candidateList, configHint);
            else if (candidateList.Count == 1)
                container.Register(typeof (ICommandBus), candidateList.First());
        }

        /// <summary>
        ///     Register all asynchronous command buses
        /// </summary>
        /// <param name="container">Container instance</param>
        protected virtual void RegisterCommandBusAsync(TinyIoCContainer container)
        {
            const string configHint = "ecf-hint:nancy-commandbus-async";

            var candidateList = FindTypes<ICommandBusAsync>().ToList();
            if (candidateList.Count > 1)
                RaiseAmbiguousTypeException<ICommandBusAsync>(candidateList, configHint);
            else if (candidateList.Count == 1)
                container.Register(typeof (ICommandBusAsync), candidateList.First());
        }

        /// <summary>
        ///     Register bounded contexts. Will find any IBoundedContext available, and register it. However if there
        ///     are multiple, an exception will be thrown. If no bounded contexts are found, then the AutoAssemblyBoundedContext
        ///     will be registered.
        /// </summary>
        /// <param name="container">Container instance</param>
        protected virtual void RegisterBoundedContexts(TinyIoCContainer container)
        {
            const string configHint = "ecf-hint:nancy-boundedcontext";

            // Get all bounded context types, but skip the ones in the ECF base library
            var candidateList = FindTypes<IBoundedContext>().Except(
                new[]
                {
                    typeof (AutoAssemblyBoundedContext)
                }
                ).ToList();

            // If we have 1 or 0 bounded context types, then we know what to do.
            if (candidateList.Count == 1)
                container.Register(typeof (IBoundedContext), candidateList.First());
            else if (candidateList.Count == 0)
                container.Register<IBoundedContext, AutoAssemblyBoundedContext>();
            else
            {
                // Fail out
                RaiseAmbiguousTypeException<IBoundedContext>(candidateList, configHint);
            }
        }

        /// <summary>
        ///     Raise the ambiguous typing exception
        /// </summary>
        /// <typeparam name="TSubjectType">Subject Type</typeparam>
        /// <param name="candidateList">Types that are assignable to TSubjectType</param>
        /// <param name="configHint">Configuration hint to allow explict setting</param>
        private static void RaiseAmbiguousTypeException<TSubjectType>(IList<Type> candidateList, string configHint)
        {
            throw new NancyEcfBootstrapException(
                string.Format(CultureInfo.CurrentUICulture, NancyMessages.AmbiguousBootstrapperType,
                    typeof (TSubjectType).FullName,
                    candidateList.Count,
                    configHint,
                    string.Join(",", candidateList.Select(_ => "'" + _.AssemblyQualifiedName + "'"))
                    )
                );
        }

        /// <summary>
        ///     Register a ITypeResolver for this instance
        /// </summary>
        /// <param name="container">Container instance</param>
        protected virtual void RegisterTypeResolver(TinyIoCContainer container)
        {
            container.Register<ITypeResolver, NancyEcfTypeResolver>();
        }

        /// <summary>
        ///     Find types implementing the specified interface.
        /// </summary>
        /// <typeparam name="TInterfaceType">Interface type to locate</typeparam>
        /// <remarks>
        ///     Types must be a class, assignable to TInterfaceType, non-abstract and not an open-generic.
        /// </remarks>
        /// <returns></returns>
        protected virtual IEnumerable<Type> FindTypes<TInterfaceType>()
        {
            return
                from assembly in GetCandidateAssemblies()
                from type in assembly.GetTypes()
                where typeof (TInterfaceType).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract && !type.IsGenericTypeDefinition
                select type;
        }

        #endregion
    }
}