namespace Sysx.EntityFramework.SqlServer.Plugins;

public static class DbContextOptionsBuilderExtensions
{
    public static TRelationalDbContextOptionsBuilderInfrastructure UseSequentialGuids<TRelationalDbContextOptionsBuilderInfrastructure>
        (this TRelationalDbContextOptionsBuilderInfrastructure dbContextOptionsBuilder)
            where TRelationalDbContextOptionsBuilderInfrastructure : IRelationalDbContextOptionsBuilderInfrastructure
    {
        EnsureArg.HasValue(dbContextOptionsBuilder, nameof(dbContextOptionsBuilder));

        dbContextOptionsBuilder.TryAddExtension(new SequentialGuidDbContextOptionsExtension());
        return dbContextOptionsBuilder;
    }
}