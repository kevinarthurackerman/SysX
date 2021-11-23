using System.Collections.Generic;
using System.Linq;
using Sysx.Linq;
using Assembly = System.Reflection.Assembly;

namespace Sysx.Reflection
{
    internal static class AssemblyLoader
    {
        internal static IEnumerable<Assembly> Load(Assembly rootAssembly)
        {
            return rootAssembly
                .Descendants(x => x.GetReferencedAssemblies().Select(y => Assembly.Load(y)))
                .ToArray();
        }
    }
}
