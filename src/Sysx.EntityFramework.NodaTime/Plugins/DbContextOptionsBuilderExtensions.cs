namespace Sysx.EntityFramework.NodaTime.Plugins;

public static class DbContextOptionsBuilderExtensions
{
    /// <summary>
    /// Adds handling of NodaTime types to EntityFramework
    /// </summary>
    public static TRelationalDbContextOptionsBuilderInfrastructure UseNodaTime<TRelationalDbContextOptionsBuilderInfrastructure>
        (this TRelationalDbContextOptionsBuilderInfrastructure dbContextOptionsBuilder)
            where TRelationalDbContextOptionsBuilderInfrastructure : IRelationalDbContextOptionsBuilderInfrastructure
    {
        EnsureArg.HasValue(dbContextOptionsBuilder, nameof(dbContextOptionsBuilder));

        dbContextOptionsBuilder.TryAddExtension(new NodaTimeDbContextOptionsExtension());
        return dbContextOptionsBuilder;
    }
}