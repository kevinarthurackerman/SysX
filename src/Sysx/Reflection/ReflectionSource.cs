namespace Sysx.Reflection;

/// <summary>
/// Contains Assemblies, Types, and filters used to select an Enumerable<Type> used for reflection based APIs.
/// </summary>
public class ReflectionSource : IEnumerable<Type>
{
    /// <summary>
    /// TypeFiler that always returns true.
    /// </summary>
    public static readonly TypeFilter Unfiltered = _ => true;

    /// <summary>
    /// The ReflectionSource containing the assembly that is the process executable in the default application domain,
    /// or the first executable that was executed by System.AppDomain.ExecuteAssembly(System.String).
    /// Can contain no assembly when called from unmanaged code.
    /// </summary>
    public static ReflectionSource GetEntrySource()
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        return entryAssembly == null
            ? new()
            : new ReflectionSource(new Assembly[] { entryAssembly });
    }

    /// <summary>
    /// Gets the ReflectionSource containing the assembly that contains the code that is currently executing.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static ReflectionSource GetExecutingSource()
    {
        var executingAssembly = new StackTrace(1).GetFrame(0)?.GetMethod()?.DeclaringType?.Assembly;
        return executingAssembly == null
            ? new()
            : new(new[] { executingAssembly });
    }

    /// <summary>
    /// Gets the ReflectionSource containing the System.Reflection.Assembly of the method that invoked the currently executing method.
    /// [MethodImpl(MethodImplOptions.NoInlining)] should be affixed to methods that call this method to ensure the proper Assembly is located.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static ReflectionSource GetCallingSource()
    {
        var executingAssembly = new StackTrace(2).GetFrame(0)?.GetMethod()?.DeclaringType?.Assembly;
        return executingAssembly == null
            ? new()
            : new(new[] { executingAssembly });
    }

    /// <summary>
    /// The FilteredAssemblies contained in this ReflectionSource.
    /// </summary>
    public readonly ICollection<FilteredAssembly> FilteredAssemblies;

    /// <summary>
    /// The Types contained in this ReflectionSource.
    /// </summary>
    public readonly ICollection<Type> Types;

    /// <summary>
    /// The TypeFilters contained in this ReflectionSource.
    /// </summary>
    public readonly ICollection<TypeFilter> TypeFilters;

    public ReflectionSource()
    {
        FilteredAssemblies = new Collection<FilteredAssembly>();
        Types = new Collection<Type>();
        TypeFilters = new Collection<TypeFilter>();
    }

    public ReflectionSource(IEnumerable<Assembly> assemblies)
    {
        EnsureArg.IsNotNull(assemblies, nameof(assemblies));
        Ensure.That(assemblies, nameof(assemblies)).DoesNotContainNull();

        FilteredAssemblies = new Collection<FilteredAssembly>(
            assemblies.Select(assembly => new FilteredAssembly(assembly, Unfiltered)).ToList());
        Types = new Collection<Type>();
        TypeFilters = new Collection<TypeFilter>();
    }

    public ReflectionSource(IEnumerable<FilteredAssembly> filteredAssemblies) : this(filteredAssemblies, Array.Empty<Type>(), Array.Empty<TypeFilter>()) { }

    public ReflectionSource(IEnumerable<Type> types) : this(Array.Empty<FilteredAssembly>(), types, Array.Empty<TypeFilter>()) { }

    public ReflectionSource(IEnumerable<FilteredAssembly> filteredAssemblies, IEnumerable<Type> types, IEnumerable<TypeFilter> filters)
    {
        EnsureArg.IsNotNull(filteredAssemblies, nameof(filteredAssemblies));
        Ensure.That(filteredAssemblies, nameof(filteredAssemblies)).DoesNotContainNull();
        EnsureArg.IsNotNull(types, nameof(types));
        Ensure.That(types, nameof(types)).DoesNotContainNull();
        EnsureArg.IsNotNull(filters, nameof(filters));
        Ensure.That(filters, nameof(filters)).DoesNotContainNull();

        FilteredAssemblies = new Collection<FilteredAssembly>(filteredAssemblies.ToList());
        Types = types.ToHashSet();
        TypeFilters = filters.ToHashSet();
    }

    /// <summary>
    /// Includes this Assembly in this ReflectionSource.
    /// </summary>
    public ReflectionSource Include(Assembly assembly) => Include(assembly, Unfiltered);

    public ReflectionSource Include(Assembly assembly, TypeFilter typeFilter)
    {
        EnsureArg.IsNotNull(assembly, nameof(assembly));
        EnsureArg.IsNotNull(typeFilter, nameof(typeFilter));

        FilteredAssemblies.Add(new FilteredAssembly(assembly, typeFilter));

        return this;
    }

    /// <inheritdoc cref="Include(IEnumerable{Assembly})" />
    public ReflectionSource Include(params Assembly[] assemblies) => Include(assemblies.AsEnumerable());

    /// <summary>
    /// Include these Assemblies in this ReflectionSource.
    /// </summary>
    public ReflectionSource Include(IEnumerable<Assembly> assemblies)
    {
        EnsureArg.IsNotNull(assemblies, nameof(assemblies));
        Ensure.That(assemblies, nameof(assemblies)).DoesNotContainNull();

        foreach (var assembly in assemblies) FilteredAssemblies.Add(new FilteredAssembly(assembly, Unfiltered));

        return this;
    }

    /// <summary>
    /// Include this FilteredAssembly in this ReflectionSource.
    /// </summary>
    public ReflectionSource Include(FilteredAssembly filteredAssembly)
    {
        EnsureArg.IsNotNull(filteredAssembly.Assembly, $"{nameof(filteredAssembly)}.{nameof(FilteredAssembly.Assembly)}");
        EnsureArg.IsNotNull(filteredAssembly.Filter, $"{nameof(filteredAssembly)}.{nameof(FilteredAssembly.Filter)}");

        FilteredAssemblies.Add(filteredAssembly);

        return this;
    }

    /// <inheritdoc cref="Include(IEnumerable{FilteredAssembly})" />
    public ReflectionSource Include(params FilteredAssembly[] filteredAssemblies) => Include(filteredAssemblies.AsEnumerable());

    /// <summary>
    /// Include these FilteredAssemblies in this ReflectionSource.
    /// </summary>
    public ReflectionSource Include(IEnumerable<FilteredAssembly> filteredAssemblies)
    {
        EnsureArg.IsNotNull(filteredAssemblies, nameof(filteredAssemblies));
        Ensure.That(filteredAssemblies.Select(x => x.Assembly), $"{nameof(filteredAssemblies)}.{nameof(FilteredAssembly.Assembly)}").DoesNotContainNull();
        Ensure.That(filteredAssemblies.Select(x => x.Filter), $"{nameof(filteredAssemblies)}.{nameof(FilteredAssembly.Filter)}").DoesNotContainNull();

        foreach (var assembly in filteredAssemblies) FilteredAssemblies.Add(assembly);

        return this;
    }

    /// <summary>
    /// Include this Type in this ReflectionSource.
    /// </summary>
    public ReflectionSource Include(Type type)
    {
        EnsureArg.IsNotNull(type, nameof(type));

        Types.Add(type);

        return this;
    }

    /// <inheritdoc cref="Include(IEnumerable{Type})" />
    public ReflectionSource Include(params Type[] types) => Include(types.AsEnumerable());

    /// <summary>
    /// Include these Types in this ReflectionSource.
    /// </summary>
    public ReflectionSource Include(IEnumerable<Type> types)
    {
        EnsureArg.IsNotNull(types, nameof(types));
        Ensure.That(types, nameof(types)).DoesNotContainNull();

        foreach (var type in types) Types.Add(type);

        return this;
    }

    /// <summary>
    /// Include this TypeFilter in this ReflectionSource.
    /// </summary>
    public ReflectionSource Include(TypeFilter filter)
    {
        EnsureArg.IsNotNull(filter, nameof(filter));

        TypeFilters.Add(filter);

        return this;
    }

    /// <inheritdoc cref="Include(IEnumerable{TypeFilter})" />
    public ReflectionSource Include(params TypeFilter[] filters) => Include(filters.AsEnumerable());

    /// <summary>
    /// Include these TypeFilters in this ReflectionSource.
    /// </summary>
    public ReflectionSource Include(IEnumerable<TypeFilter> filters)
    {
        EnsureArg.IsNotNull(filters, nameof(filters));
        Ensure.That(filters, nameof(filters)).DoesNotContainNull();

        foreach (var filter in filters) TypeFilters.Add(filter);

        return this;
    }

    public IEnumerator<Type> GetEnumerator() =>
        FilteredAssemblies
            .SelectMany(filteredAssembly => filteredAssembly.Assembly.GetTypes().Where(type => filteredAssembly.Filter(type)))
            .Concat(Types)
            .Where(type => TypeFilters.All(filter => filter(type)))
            .Distinct()
            .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static implicit operator ReflectionSource(FilteredAssembly assembly) => new(new[] { assembly });
                  
    public static implicit operator ReflectionSource(FilteredAssembly[] filteredAssemblies) => new(filteredAssemblies);
    public static implicit operator ReflectionSource(List<FilteredAssembly> filteredAssemblies) => new(filteredAssemblies);
    public static implicit operator ReflectionSource(LinkedList<FilteredAssembly> filteredAssemblies) => new(filteredAssemblies);
    public static implicit operator ReflectionSource(Collection<FilteredAssembly> filteredAssemblies) => new(filteredAssemblies);
    public static implicit operator ReflectionSource(Queue<FilteredAssembly> filteredAssemblies) => new(filteredAssemblies);
    public static implicit operator ReflectionSource(HashSet<FilteredAssembly> filteredAssemblies) => new(filteredAssemblies);
    public static implicit operator ReflectionSource(Stack<FilteredAssembly> filteredAssemblies) => new(filteredAssemblies);
    public static implicit operator ReflectionSource(ReadOnlyCollection<FilteredAssembly> filteredAssemblies) => new(filteredAssemblies);
    public static implicit operator ReflectionSource(ImmutableArray<FilteredAssembly> filteredAssemblies) => new(filteredAssemblies);
    public static implicit operator ReflectionSource(ImmutableList<FilteredAssembly> filteredAssemblies) => new(filteredAssemblies);
    public static implicit operator ReflectionSource(ImmutableQueue<FilteredAssembly> filteredAssemblies) => new(filteredAssemblies);
    public static implicit operator ReflectionSource(ImmutableHashSet<FilteredAssembly> filteredAssemblies) => new(filteredAssemblies);
    public static implicit operator ReflectionSource(ImmutableStack<FilteredAssembly> filteredAssemblies) => new(filteredAssemblies);
                  
    public static implicit operator ReflectionSource(Assembly assembly) => new(new[] { assembly });
                                         
    public static implicit operator ReflectionSource(Assembly[] assemblies) => new(assemblies);
    public static implicit operator ReflectionSource(List<Assembly> assemblies) => new(assemblies);
    public static implicit operator ReflectionSource(LinkedList<Assembly> assemblies) => new(assemblies);
    public static implicit operator ReflectionSource(Collection<Assembly> assemblies) => new(assemblies);
    public static implicit operator ReflectionSource(Queue<Assembly> assemblies) => new(assemblies);
    public static implicit operator ReflectionSource(HashSet<Assembly> assemblies) => new(assemblies);
    public static implicit operator ReflectionSource(Stack<Assembly> assemblies) => new(assemblies);
    public static implicit operator ReflectionSource(ReadOnlyCollection<Assembly> assemblies) => new(assemblies);
    public static implicit operator ReflectionSource(ImmutableArray<Assembly> assemblies) => new(assemblies);
    public static implicit operator ReflectionSource(ImmutableList<Assembly> assemblies) => new(assemblies);
    public static implicit operator ReflectionSource(ImmutableQueue<Assembly> assemblies) => new(assemblies);
    public static implicit operator ReflectionSource(ImmutableHashSet<Assembly> assemblies) => new(assemblies);
    public static implicit operator ReflectionSource(ImmutableStack<Assembly> assemblies) => new(assemblies);
                                         
    public static implicit operator ReflectionSource(Type type) => new(new[] { type });
                                         
    public static implicit operator ReflectionSource(Type[] types) => new(types);
    public static implicit operator ReflectionSource(List<Type> types) => new(types);
    public static implicit operator ReflectionSource(LinkedList<Type> types) => new(types);
    public static implicit operator ReflectionSource(Collection<Type> types) => new(types);
    public static implicit operator ReflectionSource(Queue<Type> types) => new(types);
    public static implicit operator ReflectionSource(HashSet<Type> types) => new(types);
    public static implicit operator ReflectionSource(Stack<Type> types) => new(types);
    public static implicit operator ReflectionSource(ReadOnlyCollection<Type> types) => new(types);
    public static implicit operator ReflectionSource(ImmutableArray<Type> types) => new(types);
    public static implicit operator ReflectionSource(ImmutableList<Type> types) => new(types);
    public static implicit operator ReflectionSource(ImmutableQueue<Type> types) => new(types);
    public static implicit operator ReflectionSource(ImmutableHashSet<Type> types) => new(types);
    public static implicit operator ReflectionSource(ImmutableStack<Type> types) => new(types);
                  
    public static implicit operator Type[](ReflectionSource reflectionSet) => reflectionSet.ToArray();
    public static implicit operator List<Type>(ReflectionSource reflectionSet) => reflectionSet.ToList();
    public static implicit operator LinkedList<Type>(ReflectionSource reflectionSet) => new(reflectionSet);
    public static implicit operator Collection<Type>(ReflectionSource reflectionSet) => new(reflectionSet.ToList());
    public static implicit operator Queue<Type>(ReflectionSource reflectionSet) => new(reflectionSet);
    public static implicit operator HashSet<Type>(ReflectionSource reflectionSet) => reflectionSet.ToHashSet();
    public static implicit operator Stack<Type>(ReflectionSource reflectionSet) => new(reflectionSet);
    public static implicit operator ReadOnlyCollection<Type>(ReflectionSource reflectionSet) => new(reflectionSet.ToList());
    public static implicit operator ImmutableArray<Type>(ReflectionSource reflectionSet) => reflectionSet.ToImmutableArray();
    public static implicit operator ImmutableList<Type>(ReflectionSource reflectionSet) => reflectionSet.ToImmutableList();
    public static implicit operator ImmutableQueue<Type>(ReflectionSource reflectionSet) => ImmutableQueue.Create(reflectionSet.ToArray());
    public static implicit operator ImmutableHashSet<Type>(ReflectionSource reflectionSet) => reflectionSet.ToImmutableHashSet();
    public static implicit operator ImmutableStack<Type>(ReflectionSource reflectionSet) => ImmutableStack.Create(reflectionSet.ToArray());

    public delegate bool TypeFilter(Type type);

    public record struct FilteredAssembly(Assembly Assembly, TypeFilter Filter);
}