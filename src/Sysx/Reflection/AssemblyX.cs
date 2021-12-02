namespace Sysx.Reflection;

/// <summary>
/// Used to recursively load the dependencies of an assembly.
/// </summary>
public static class AssemblyX
{
    /// <summary>
    /// Recursively loads the dependencies of an assembly.
    /// </summary>
    public static IEnumerable<Assembly> LoadDependencies(Assembly rootAssembly)
    {
        EnsureArg.IsNotNull(rootAssembly, nameof(rootAssembly));

        return rootAssembly
            .Descendants(x => x.GetReferencedAssemblies().Select(y => Assembly.Load(y)));
    }
}