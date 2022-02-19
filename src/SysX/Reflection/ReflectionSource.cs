using System.Collections.Immutable;

namespace SysX.Reflection;

/// <summary>
/// Contains <see cref="Assembly"/>s, <see cref="Type"/>s, and <see cref="TypeFilter"/>s used to select an <see cref="IEnumerable{T}"/> of <see cref="Type"/> used for reflection based APIs.
/// </summary>
public class ReflectionSource : IEnumerable<Type>
{
    /// <summary>
    /// TypeFiler that always returns true.
    /// </summary>
    public static readonly TypeFilter Unfiltered = _ => true;

    /// <summary>
    /// The <see cref="ReflectionSource"/> containing the <see cref="Assembly"/> that is the process executable in the default <see cref="AppDomain"/>,
    /// or the first executable that was executed by <see cref="AppDomain.ExecuteAssembly(string)"/>.
    /// Can contain no <see cref="Assembly"/> when called from unmanaged code.
    /// </summary>
    public static ReflectionSource GetEntrySource()
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        return entryAssembly == null
            ? new()
            : new ReflectionSource(new Assembly[] { entryAssembly });
    }

    /// <summary>
    /// Gets the <see cref="ReflectionSource"/> containing the <see cref="Assembly"/> that contains the code that is currently executing.
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
    /// Gets the <see cref="ReflectionSource"/> containing the <see cref="Assembly"/> of the method that invoked the currently executing method.
    /// [<see cref="MethodImplAttribute"/>(<see cref="MethodImplOptions.NoInlining"/>)] should be affixed to methods that call this method to ensure the proper <see cref="Assembly"/> is located.
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
    /// The <see cref="FilteredAssembly"/>s contained in this <see cref="ReflectionSource"/>.
    /// </summary>
    public readonly ICollection<FilteredAssembly> FilteredAssemblies;

    /// <summary>
    /// The <see cref="Type"/>s contained in this <see cref="ReflectionSource"/>.
    /// </summary>
    public readonly ICollection<Type> Types;

    /// <summary>
    /// The <see cref="TypeFilter"/>s contained in this <see cref="ReflectionSource"/>.
    /// </summary>
    public readonly ICollection<TypeFilter> TypeFilters;

    /// <summary>
    /// Initializes a new empty <see cref="ReflectionSource"/>.
    /// </summary>
    public ReflectionSource()
    {
        FilteredAssemblies = new Collection<FilteredAssembly>();
        Types = new Collection<Type>();
        TypeFilters = new Collection<TypeFilter>();
    }

    /// <summary>
    /// Initializes a <see cref="ReflectionSource"/> with the given <see cref="Assembly"/>s.
    /// </summary>
    public ReflectionSource(IEnumerable<Assembly> assemblies)
    {
        EnsureArg.IsNotNull(assemblies, nameof(assemblies));
        Ensure.That(assemblies, nameof(assemblies)).DoesNotContainNull();

        FilteredAssemblies = new Collection<FilteredAssembly>(
            assemblies.Select(assembly => new FilteredAssembly(assembly, Unfiltered)).ToList());
        Types = new Collection<Type>();
        TypeFilters = new Collection<TypeFilter>();
    }

    /// <summary>
    /// Initializes a <see cref="ReflectionSource"/> with the given <see cref="FilteredAssembly"/>s.
    /// </summary>
    public ReflectionSource(IEnumerable<FilteredAssembly> filteredAssemblies)
        : this(filteredAssemblies, Array.Empty<Type>(), Array.Empty<TypeFilter>()) { }

    /// <summary>
    /// Initializes a <see cref="ReflectionSource"/> with the given <see cref="Type"/>s.
    /// </summary>
    public ReflectionSource(IEnumerable<Type> types)
        : this(Array.Empty<FilteredAssembly>(), types, Array.Empty<TypeFilter>()) { }

    /// <summary>
    /// Initializes a <see cref="ReflectionSource"/> with the given <see cref="FilteredAssembly"/>s, <see cref="Types"/>s, and <see cref="TypeFilter"/>s.
    /// </summary>
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
    /// Adds the <see cref="FilteredAssembly"/>s, <see cref="Types"/>s, and <see cref="TypeFilter"/>s of the given <see cref="ReflectionSource"/> in this <see cref="ReflectionSource"/>.
    /// </summary>
    public ReflectionSource Include(ReflectionSource reflectionSource)
    {
        EnsureArg.IsNotNull(reflectionSource, nameof(reflectionSource));

        foreach (var filteredAssembly in reflectionSource.FilteredAssemblies) FilteredAssemblies.Add(filteredAssembly);
        foreach (var type in reflectionSource.Types) Types.Add(type);
        foreach (var typeFilter in reflectionSource.TypeFilters) TypeFilters.Add(typeFilter);

        return this;
    }

    /// <inheritdoc cref="Include(IEnumerable{ReflectionSource})" />
    public ReflectionSource Include(params ReflectionSource[] reflectionSources) => Include(reflectionSources.AsEnumerable());

    /// <summary>
    /// Adds the <see cref="FilterAssembly"/>s, <see cref="Type"/>s, and <see cref="TypeFilter"/>s of the given <see cref="ReflectionSource"/>s in this <see cref="ReflectionSource"/>.
    /// </summary>
    public ReflectionSource Include(IEnumerable<ReflectionSource> reflectionSources)
    {
        EnsureArg.IsNotNull(reflectionSources, nameof(reflectionSources));
        Ensure.That(reflectionSources, nameof(reflectionSources)).DoesNotContainNull();

        foreach (var filteredAssembly in reflectionSources.SelectMany(x => x.FilteredAssemblies)) FilteredAssemblies.Add(filteredAssembly);
        foreach (var type in reflectionSources.SelectMany(x => x.Types)) Types.Add(type);
        foreach (var typeFilter in reflectionSources.SelectMany(x => x.TypeFilters)) TypeFilters.Add(typeFilter);

        return this;
    }

    /// <summary>
    /// Includes the given <see cref="Assembly"/> in this <see cref="ReflectionSource"/>.
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
    /// Include the given <see cref="Assembly"/>s in this <see cref="ReflectionSource"/>.
    /// </summary>
    public ReflectionSource Include(IEnumerable<Assembly> assemblies)
    {
        EnsureArg.IsNotNull(assemblies, nameof(assemblies));
        Ensure.That(assemblies, nameof(assemblies)).DoesNotContainNull();

        foreach (var assembly in assemblies) FilteredAssemblies.Add(new FilteredAssembly(assembly, Unfiltered));

        return this;
    }

    /// <summary>
    /// Include the given <see cref="FilteredAssembly"/> in this <see cref="ReflectionSource"/>.
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
    /// Include the given <see cref="FilteredAssembly"/>s in this <see cref="ReflectionSource"/>.
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
    /// Include the given <see cref="Type"/> in this <see cref="ReflectionSource"/>.
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
    /// Include the given <see cref="Type"/>s in this <see cref="ReflectionSource"/>.
    /// </summary>
    public ReflectionSource Include(IEnumerable<Type> types)
    {
        EnsureArg.IsNotNull(types, nameof(types));
        Ensure.That(types, nameof(types)).DoesNotContainNull();

        foreach (var type in types) Types.Add(type);

        return this;
    }

    /// <summary>
    /// Include the given <see cref="TypeFilter"/> in this <see cref="ReflectionSource"/>.
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
    /// Include the given <see cref="TypeFilters"/>s in this <see cref="ReflectionSource"/>.
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

    public static ReflectionSource operator +(ReflectionSource left, ReflectionSource right) => left.Include(right);

    /// <summary>
    /// A filter <see cref="Func{T, TResult}"/> that takes a <see cref="Type"/> and returns a <see cref="bool"/> indicating if it should be included in the source.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public delegate bool TypeFilter(Type type);

    /// <summary>
    /// An <see cref="Assembly"/> with a <see cref="TypeFilter"/> for selecting a subset of all provided <see cref="Type"/>s.
    /// </summary>
    public record struct FilteredAssembly(Assembly Assembly, TypeFilter Filter);
}