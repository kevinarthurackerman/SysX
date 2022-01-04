namespace Sysx.EntityFramework.Plugins;

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
    /// Adds handling of sequential GUID types to EntityFramework
    /// </summary>
    public static TRelationalDbContextOptionsBuilderInfrastructure UseSequentialGuids<TRelationalDbContextOptionsBuilderInfrastructure>
        (this TRelationalDbContextOptionsBuilderInfrastructure dbContextOptionsBuilder)
            where TRelationalDbContextOptionsBuilderInfrastructure : IRelationalDbContextOptionsBuilderInfrastructure
    {
        EnsureArg.HasValue(dbContextOptionsBuilder, nameof(dbContextOptionsBuilder));

        dbContextOptionsBuilder.TryAddExtension(new SequentialGuidDbContextOptionsExtension());
        return dbContextOptionsBuilder;
    }

    /// <inheritdoc cref="UseEnumerationsByDisplayName{TRelationalDbContextOptionsBuilderInfrastructure}(TRelationalDbContextOptionsBuilderInfrastructure, Assembly)"/>
    public static TRelationalDbContextOptionsBuilderInfrastructure UseEnumerationsByDisplayName<TRelationalDbContextOptionsBuilderInfrastructure>
        (this TRelationalDbContextOptionsBuilderInfrastructure dbContextOptionsBuilder)
            where TRelationalDbContextOptionsBuilderInfrastructure : IRelationalDbContextOptionsBuilderInfrastructure
            => UseEnumerationsByDisplayName(dbContextOptionsBuilder, Assembly.GetCallingAssembly());

    /// <summary>
    /// Adds handling of enumeration types by mapping to their display name to EntityFramework
    /// </summary>
    public static TRelationalDbContextOptionsBuilderInfrastructure UseEnumerationsByDisplayName<TRelationalDbContextOptionsBuilderInfrastructure>
        (this TRelationalDbContextOptionsBuilderInfrastructure dbContextOptionsBuilder, Assembly scanAssembly)
            where TRelationalDbContextOptionsBuilderInfrastructure : IRelationalDbContextOptionsBuilderInfrastructure
    {
        EnsureArg.HasValue(dbContextOptionsBuilder, nameof(dbContextOptionsBuilder));
        EnsureArg.IsNotNull(scanAssembly, nameof(scanAssembly));

        dbContextOptionsBuilder.TryAddExtension(new BaseEnumerationsByDisplayNameDbContextOptionsExtension(scanAssembly));
        return dbContextOptionsBuilder;
    }

    /// <inheritdoc cref="UseEnumerationsByValue{TRelationalDbContextOptionsBuilderInfrastructure}(TRelationalDbContextOptionsBuilderInfrastructure, Assembly)"/>
    public static TRelationalDbContextOptionsBuilderInfrastructure UseEnumerationsByValue<TRelationalDbContextOptionsBuilderInfrastructure>
        (this TRelationalDbContextOptionsBuilderInfrastructure dbContextOptionsBuilder)
            where TRelationalDbContextOptionsBuilderInfrastructure : IRelationalDbContextOptionsBuilderInfrastructure
            => dbContextOptionsBuilder.UseEnumerationsByValue(Assembly.GetCallingAssembly());

    /// <summary>
    /// Adds handling of enumeration types by mapping to their value type to EntityFramework
    /// </summary>
    public static TRelationalDbContextOptionsBuilderInfrastructure UseEnumerationsByValue<TRelationalDbContextOptionsBuilderInfrastructure>
        (this TRelationalDbContextOptionsBuilderInfrastructure dbContextOptionsBuilder, Assembly scanAssembly)
            where TRelationalDbContextOptionsBuilderInfrastructure : IRelationalDbContextOptionsBuilderInfrastructure
    {
        EnsureArg.HasValue(dbContextOptionsBuilder, nameof(dbContextOptionsBuilder));
        EnsureArg.IsNotNull(scanAssembly, nameof(scanAssembly));

        dbContextOptionsBuilder.TryAddExtension(new BaseEnumerationsByValueDbContextOptionsExtension(scanAssembly));
        return dbContextOptionsBuilder;
    }
}