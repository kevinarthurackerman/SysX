namespace Sysx.Reflection;

#if NET5_0 || NETCOREAPP3_1
using System.Runtime.Loader;
#endif

/// <summary>
/// Used to recursively load the dependencies of an assembly.
/// </summary>
public static class AssemblyX
{
#if NETSTANDARD2_1 || NET48
    /// <summary>
    /// Recursively loads the dependencies of an assembly.
    /// </summary>
    public static IEnumerable<Assembly> Load()
    {
        var rootAssembly = Assembly.GetEntryAssembly();

        EnsureArg.IsNotNull(rootAssembly);

        return rootAssembly.Descendants(x => x.GetReferencedAssemblies().Select(y => Assembly.Load(y)));
    }

#elif NET5_0 || NETCOREAPP3_1
    private static readonly LoadOptions DefaultLoadOptions = LoadOptions.Default;

    /// <inheritdoc cref="Load(LoadOptions)"/>
    public static IEnumerable<Assembly> Load() => Load(in DefaultLoadOptions);

    /// <summary>
    /// Recursively loads the dependencies of an assembly.
    /// </summary>
    public static IEnumerable<Assembly> Load(in LoadOptions options)
    {
        options.Validate();

        var rootAssembly = ResolveAssembly(options.RootAssemblyName!, in options);

        var maxDepth = options.LoadDepth == LoadDepth.LoadRootOnly ? 0
            : options.LoadDepth == LoadDepth.LoadChildren ? 1
            : options.LoadDepth == LoadDepth.RecursivelyLoadDescendants ? int.MaxValue
            : throw new InvalidOperationException();
        
        var copyOptions = options;

        return rootAssembly.Descendants(
            x => x.GetReferencedAssemblies().Select(assemblyName => ResolveAssembly(assemblyName, copyOptions)),
            includeRoot: options.IncludeRootAssembly,
            maxDepth: maxDepth
        );
        
        Assembly ResolveAssembly(AssemblyName assemblyName, in LoadOptions options)
        {
            var assembly = options.AssemblyLoadContext!.Assemblies.FirstOrDefault(x => x.GetName().FullName == assemblyName.FullName);

            if (assembly != null) return assembly;

            var assemblyPath = options.AssemblyDependencyResolvers!
                .Select(resolver => resolver.ResolveAssemblyToPath(assemblyName))
                .Where(path => path != null)
                .FirstOrDefault();
            
            EnsureArg.IsNotNull(assemblyPath);

            using (options.AssemblyLoadContext!.EnterContextualReflection())
            {
                return Assembly.LoadFile(assemblyPath);
            }
        }
    }

    public record struct LoadOptions
    {
        public static readonly AssemblyDependencyResolver FrameworkDependencyResolver =
            new (typeof(object).Assembly.Location);

        public static readonly AssemblyDependencyResolver CurrentDomainAssemblyDependencyResolver =
            new (AppDomain.CurrentDomain.BaseDirectory);

        public static LoadOptions Default => new()
        {
            IncludeRootAssembly = true,
            RootAssemblyName = Assembly.GetEntryAssembly()?.GetName(),
            AssemblyDependencyResolvers = new List<AssemblyDependencyResolver>{ FrameworkDependencyResolver, CurrentDomainAssemblyDependencyResolver },
            AssemblyLoadContext = AssemblyLoadContext.Default,
            LoadDepth = LoadDepth.LoadRootOnly
        };

        public bool IncludeRootAssembly { get; set; }
        public AssemblyName? RootAssemblyName { get; set; }
        public IList<AssemblyDependencyResolver>? AssemblyDependencyResolvers { get; set; }
        public AssemblyLoadContext? AssemblyLoadContext { get; set; }
        public LoadDepth LoadDepth { get; set; }

        public void Validate()
        {
            EnsureArg.IsNotNull(RootAssemblyName, nameof(RootAssemblyName));
            EnsureArg.IsNotNull(AssemblyDependencyResolvers, nameof(AssemblyDependencyResolvers));
            EnsureArg.HasItems(AssemblyDependencyResolvers, nameof(AssemblyDependencyResolvers));
            EnsureArg.IsNotNull(AssemblyLoadContext, nameof(AssemblyLoadContext));
            EnsureArg.EnumIsDefined(LoadDepth, nameof(LoadDepth));
        }
    }

    public enum LoadDepth
    {
        LoadRootOnly = 1,
        LoadChildren = 2,
        RecursivelyLoadDescendants = 3
    }
#endif
}