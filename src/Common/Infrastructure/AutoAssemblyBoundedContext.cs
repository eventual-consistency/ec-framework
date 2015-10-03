using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EventualConsistency.Framework.Infrastructure
{
    /// <summary>
    ///     The AutoAssemblyBoundedContext class allows us to specify a number of assemblies in the
    ///     applications probing path and then use the types therein as an automatically built
    ///     bounded context, simplifying some aspects of applicaton development.
    /// </summary>
    public class AutoAssemblyBoundedContext : IBoundedContext
    {

        #region Constructor(s)

        /// <summary>
        ///     Initialize a new instance, using the root application name as the domain
        ///     and a the loaded assemblies as the start point.
        /// </summary>
        public AutoAssemblyBoundedContext()
        {
            var events = new List<Type>();
            var faults = new List<Type>();
            var commands = new List<Type>();

            ContextName = AppDomain.CurrentDomain.FriendlyName;

            var list = new List<Assembly>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                assembly.LoadReferencesRecursive(list);

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                RegisterTypesInAssembly(assembly, commands, events, faults);


            CommandTypes = commands;
            EventTypes = events;
            FaultTypes = faults;
        }

        /// <summary>
        ///     Initialize a new instance
        /// </summary>
        public AutoAssemblyBoundedContext(string name, string[] assemblyNames)
        {
            ContextName = name;
            LocateTypes(assemblyNames);
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Context Name
        /// </summary>
        public string ContextName { get; }

        /// <summary>
        ///     Command Types
        /// </summary>
        public IEnumerable<Type> CommandTypes { get; private set; }

        /// <summary>
        ///     Event Types
        /// </summary>
        public IEnumerable<Type> EventTypes { get; private set; }

        /// <summary>
        ///     Fault Types
        /// </summary>
        public IEnumerable<Type> FaultTypes { get; private set; }

        #endregion


        #region Methods

        /// <summary>
        ///     Locate types in the specified assemblies
        /// </summary>
        /// <param name="assemblies">Assemblies</param>
        protected void LocateTypes(string[] assemblies)
        {
            var events = new List<Type>();
            var faults = new List<Type>();
            var commands = new List<Type>();

            foreach (var assemblyName in assemblies)
            {
                var currentAssembly = AppDomain.CurrentDomain.Load(assemblyName);
                RegisterTypesInAssembly(currentAssembly, commands, events, faults);
            }

            CommandTypes = commands;
            EventTypes = events;
            FaultTypes = faults;
        }

        /// <summary>
        ///     Locate types in the application and all related assemblies.
        /// </summary>
        protected void LocateTypesInAllAssemblies()
        {
            var events = new List<Type>();
            var faults = new List<Type>();
            var commands = new List<Type>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                RegisterTypesInAssembly(assembly, commands, events, faults);

            CommandTypes = commands;
            EventTypes = events;
            FaultTypes = faults;
        }


        /// <summary>
        ///     Register types in assembly
        /// </summary>
        /// <param name="currentAssembly">Assembly to load values for</param>
        /// <param name="commands">Commmands</param>
        /// <param name="events">Events</param>
        /// <param name="faults">Faults</param>

        private static void RegisterTypesInAssembly(Assembly currentAssembly, List<Type> commands, List<Type> events, List<Type> faults)
        {
            commands.AddRange(currentAssembly.GetTypes().Where(x => typeof (ICommand).IsAssignableFrom(x)));
            events.AddRange(currentAssembly.GetTypes().Where(x => typeof (IEvent).IsAssignableFrom(x)));
            faults.AddRange(currentAssembly.GetTypes().Where(x => typeof (IDomainFault).IsAssignableFrom(x)));
        }

        #endregion
    }
}