namespace Sysx.EntityFramework.Sqlite.NodaTime.Plugins;

public static class DbContextOptionsBuilderExtensions
{
    public static TRelationalDbContextOptionsBuilderInfrastructure UseNodaTime<TRelationalDbContextOptionsBuilderInfrastructure>
        (this TRelationalDbContextOptionsBuilderInfrastructure dbContextOptionsBuilder)
            where TRelationalDbContextOptionsBuilderInfrastructure : IRelationalDbContextOptionsBuilderInfrastructure
    {
        EnsureArg.HasValue(dbContextOptionsBuilder, nameof(dbContextOptionsBuilder));

        dbContextOptionsBuilder.TryAddExtension(new NodaTimeDbContextOptionsExtension());
        return dbContextOptionsBuilder;
    }
}