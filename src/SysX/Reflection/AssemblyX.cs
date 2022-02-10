namespace SysX.Reflection;

/// <summary>
/// Used to recursively load the dependencies of an <see cref="Assembly"/>.
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

        return EnumerableX.Descendants(rootAssembly, x => x.GetReferencedAssemblies().Select(y => Assembly.Load(y)));
    }

#elif NET6_0 || NET5_0 || NETCOREAPP3_1
    private static readonly LoadOptions defaultLoadOptions = LoadOptions.Default;

    /// <inheritdoc cref="Load(LoadOptions)"/>
    public static IEnumerable<Assembly> Load() => Load(in defaultLoadOptions);

    /// <summary>
    /// Recursively loads the dependencies of an <see cref="Assembly"/>.
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

        return EnumerableX.Descendants(rootAssembly,
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

    /// <summary>
    /// Options for loading <see cref="Assembly"/>s.
    /// </summary>
    public record struct LoadOptions
    {
        /// <summary>
        /// A <see cref="AssemblyDependencyResolver"/> framework types.
        /// </summary>
        public static readonly AssemblyDependencyResolver FrameworkDependencyResolver =
            new (typeof(object).Assembly.Location);

        /// <summary>
        /// A <see cref="AssemblyDependencyResolver"/> for the current domain.
        /// </summary>
        public static readonly AssemblyDependencyResolver CurrentDomainAssemblyDependencyResolver =
            new (AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory ?? AppDomain.CurrentDomain.SetupInformation.ApplicationBase!);

        /// <summary>
        /// Default value to use for <see cref="LoadOptions"/>.
        /// </summary>
        public static LoadOptions Default => new()
        {
            IncludeRootAssembly = true,
            RootAssemblyName = Assembly.GetEntryAssembly()?.GetName(),
            AssemblyDependencyResolvers = new List<AssemblyDependencyResolver>{ FrameworkDependencyResolver, CurrentDomainAssemblyDependencyResolver },
            AssemblyLoadContext = AssemblyLoadContext.Default,
            LoadDepth = LoadDepth.LoadRootOnly
        };

        /// <summary>
        /// The output will include the root <see cref="Assembly"/> value when <see langword="true"/>.
        /// </summary>
        public bool IncludeRootAssembly { get; set; }

        /// <summary>
        /// The root <see cref="AssemblyName"/> to use when resolving <see cref="Assembly"/>s.
        /// </summary>
        public AssemblyName? RootAssemblyName { get; set; }

        /// <summary>
        /// <see cref="AssemblyDependencyResolver"/>s to use when resolving the dependencies of an <see cref="Assembly"/>.
        /// </summary>
        public IList<AssemblyDependencyResolver>? AssemblyDependencyResolvers { get; set; }

        /// <summary>
        /// <see cref="AssemblyLoadContext"/> to use when loading an <see cref="Assembly"/>.
        /// </summary>
        public AssemblyLoadContext? AssemblyLoadContext { get; set; }

        /// <summary>
        /// Indicates if the root, it's immediate children, or all descendants should be loaded.
        /// </summary>
        public LoadDepth LoadDepth { get; set; }

        /// <summary>
        /// Validates this options configuration.
        /// </summary>
        public void Validate()
        {
            EnsureArg.IsNotNull(RootAssemblyName, nameof(RootAssemblyName));
            EnsureArg.IsNotNull(AssemblyDependencyResolvers, nameof(AssemblyDependencyResolvers));
            EnsureArg.HasItems(AssemblyDependencyResolvers, nameof(AssemblyDependencyResolvers));
            EnsureArg.IsNotNull(AssemblyLoadContext, nameof(AssemblyLoadContext));
            EnsureArg.EnumIsDefined(LoadDepth, nameof(LoadDepth));
        }
    }

    /// <summary>
    /// Indicates if the root, it's immediate children, or all descendants should be loaded.
    /// </summary>
    public enum LoadDepth
    {
        LoadRootOnly = 1,
        LoadChildren = 2,
        RecursivelyLoadDescendants = 3
    }
#endif
}