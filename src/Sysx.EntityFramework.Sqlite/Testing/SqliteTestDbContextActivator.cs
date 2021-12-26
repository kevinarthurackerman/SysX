namespace Sysx.EntityFramework.Sqlite.Testing;

public sealed class SqliteTestDbContextActivator : BaseTestDbContextActivator
{
    private static readonly SqliteTestDbContextActivator instance = new();

    private SqliteTestDbContextActivator() { }

    /// <inheritdoc cref="BaseTestDbContextActivator.CreateDbContext{TDbContext}" />
    public static TDbContext Create<TDbContext>() where TDbContext : DbContext => instance.CreateDbContext<TDbContext>();

    override protected string GetDatabaseName(Type dbContextType)
    {
        return $"{dbContextType.Name}_{Guid.NewGuid()}.sqlite";
    }

    override protected DbConnection CreateConnection(string databasePath)
    {
        var connectionString = $"Data Source={databasePath};";
        return new SqliteConnection(connectionString);
    }

    override protected DbContextOptions<TDbContext> CreateDbContextOptions<TDbContext>(DbConnection dbConnection)
    {
        return new DbContextOptionsBuilder<TDbContext>()
                .UseSqlite(dbConnection)
                .Options;
    }

    override protected void CreateDatabase(string databasePath)
    {
        File.WriteAllBytes(databasePath, Array.Empty<byte>());
    }
}
