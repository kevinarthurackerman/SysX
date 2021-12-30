namespace Sysx.EntityFramework.Sqlite.Plugins;

public static class DbContextOptionsBuilderExtensions
{
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
}