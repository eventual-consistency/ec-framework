using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EventualConsistency.Framework.Infrastructure
{
    /// <summary>
    ///     Assembly extension methods
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        ///     Load assembly references recursive
        /// </summary>
        /// <param name="assembly">Assembly to recurse</param>
        /// <param name="loadedAssemblies">Assemblies</param>
        public static void LoadReferencesRecursive(this Assembly assembly, List<Assembly> loadedAssemblies)
        {
            foreach (var namedAssembly in assembly.GetReferencedAssemblies())
            {
                var loaded = Assembly.Load(namedAssembly);
                if (!loadedAssemblies.Contains(loaded))
                {
                    loadedAssemblies.Add(loaded);
                    loaded.LoadReferencesRecursive(loadedAssemblies);
                }
            }
        }
    }
}
