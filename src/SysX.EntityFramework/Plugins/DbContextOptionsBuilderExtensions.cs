namespace SysX.EntityFramework.Plugins;

public static class DbContextOptionsBuilderExtensions
{
    /// <summary>
    /// Checks if the extension already exists and adds it if it does not.
    /// </summary>
    public static bool TryAddExtension<TExtension>(this IRelationalDbContextOptionsBuilderInfrastructure dbContextOptionsBuilder, TExtension extension)
        where TExtension : class, IDbContextOptionsExtension
    {
        EnsureArg.IsNotNull(dbContextOptionsBuilder, nameof(dbContextOptionsBuilder));
        EnsureArg.IsNotNull(extension, nameof(extension));

        var coreOptionsBuilder = dbContextOptionsBuilder.OptionsBuilder;

        if (coreOptionsBuilder.Options.FindExtension<TExtension>() == null)
        {
            ((IDbContextOptionsBuilderInfrastructure)coreOptionsBuilder).AddOrUpdateExtension(extension);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Adds handling of identifier types to EntityFramework
    /// </summary>
    public static TRelationalDbContextOptionsBuilderInfrastructure UseIdentifiers<TRelationalDbContextOptionsBuilderInfrastructure>
        (this TRelationalDbContextOptionsBuilderInfrastructure dbContextOptionsBuilder)
            where TRelationalDbContextOptionsBuilderInfrastructure : IRelationalDbContextOptionsBuilderInfrastructure
    {
        EnsureArg.HasValue(dbContextOptionsBuilder, nameof(dbContextOptionsBuilder));

        dbContextOptionsBuilder.TryAddExtension(new IdentifiersDbContextOptionsExtension());
        return dbContextOptionsBuilder;
    }

    /// <inheritdoc cref="UseEnumerationsByDisplayName{TRelationalDbContextOptionsBuilderInfrastructure}(TRelationalDbContextOptionsBuilderInfrastructure, ReflectionSource)"/>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static TRelationalDbContextOptionsBuilderInfrastructure UseEnumerationsByDisplayName<TRelationalDbContextOptionsBuilderInfrastructure>
        (this TRelationalDbContextOptionsBuilderInfrastructure dbContextOptionsBuilder)
            where TRelationalDbContextOptionsBuilderInfrastructure : IRelationalDbContextOptionsBuilderInfrastructure
            => UseEnumerationsByDisplayName(dbContextOptionsBuilder, ReflectionSource.GetCallingSource());

    /// <summary>
    /// Adds handling of enumeration types by mapping to their display name to EntityFramework
    /// </summary>
    public static TRelationalDbContextOptionsBuilderInfrastructure UseEnumerationsByDisplayName<TRelationalDbContextOptionsBuilderInfrastructure>
        (this TRelationalDbContextOptionsBuilderInfrastructure dbContextOptionsBuilder, ReflectionSource reflectionSource)
            where TRelationalDbContextOptionsBuilderInfrastructure : IRelationalDbContextOptionsBuilderInfrastructure
    {
        EnsureArg.HasValue(dbContextOptionsBuilder, nameof(dbContextOptionsBuilder));
        EnsureArg.IsNotNull(reflectionSource, nameof(reflectionSource));

        dbContextOptionsBuilder.TryAddExtension(new BaseEnumerationsByDisplayNameDbContextOptionsExtension(reflectionSource));
        return dbContextOptionsBuilder;
    }

    /// <inheritdoc cref="UseEnumerationsByValue{TRelationalDbContextOptionsBuilderInfrastructure}(TRelationalDbContextOptionsBuilderInfrastructure, ReflectionSource)"/>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static TRelationalDbContextOptionsBuilderInfrastructure UseEnumerationsByValue<TRelationalDbContextOptionsBuilderInfrastructure>
        (this TRelationalDbContextOptionsBuilderInfrastructure dbContextOptionsBuilder)
            where TRelationalDbContextOptionsBuilderInfrastructure : IRelationalDbContextOptionsBuilderInfrastructure
            => dbContextOptionsBuilder.UseEnumerationsByValue(ReflectionSource.GetCallingSource());

    /// <summary>
    /// Adds handling of enumeration types by mapping to their value type to EntityFramework
    /// </summary>
    public static TRelationalDbContextOptionsBuilderInfrastructure UseEnumerationsByValue<TRelationalDbContextOptionsBuilderInfrastructure>
        (this TRelationalDbContextOptionsBuilderInfrastructure dbContextOptionsBuilder, ReflectionSource reflectionSource)
            where TRelationalDbContextOptionsBuilderInfrastructure : IRelationalDbContextOptionsBuilderInfrastructure
    {
        EnsureArg.HasValue(dbContextOptionsBuilder, nameof(dbContextOptionsBuilder));
        EnsureArg.IsNotNull(reflectionSource, nameof(reflectionSource));

        dbContextOptionsBuilder.TryAddExtension(new BaseEnumerationsByValueDbContextOptionsExtension(reflectionSource));
        return dbContextOptionsBuilder;
    }
}