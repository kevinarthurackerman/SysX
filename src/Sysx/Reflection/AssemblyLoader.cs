using System.Collections.Generic;
using System.Linq;
using Sysx.Linq;
using Assembly = System.Reflection.Assembly;

namespace Sysx.Reflection
{
    /// <summary>
    /// Used to recursively load the dependencies of an assembly.
    /// </summary>
    internal static class AssemblyLoader
    {
        /// <summary>
        /// Recursively loads the dependencies of an assembly.
        /// </summary>
        internal static IEnumerable<Assembly> LoadDependencies(Assembly rootAssembly)
        {
            return rootAssembly
                .Descendants(x => x.GetReferencedAssemblies().Select(y => Assembly.Load(y)));
        }
    }
}
