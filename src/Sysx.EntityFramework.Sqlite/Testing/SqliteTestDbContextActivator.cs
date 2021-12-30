namespace Sysx.EntityFramework.Sqlite.Testing;

public sealed class SqliteTestDbContextActivator : BaseTestDbContextActivator<SqliteDbContextOptionsBuilder>
{
    private static readonly SqliteTestDbContextActivator instance = new();

    private SqliteTestDbContextActivator() { }

    /// <inheritdoc cref="BaseTestDbContextActivator.CreateDbContext{TDbContext}(Action{DbContextOptionsBuilder{TDbContext}}?, Action{TDbContextOptionsBuilder}?)"/>
    public static TDbContext Create<TDbContext>() where TDbContext : DbContext =>
        instance.CreateDbContext<TDbContext>();

    /// <inheritdoc cref="BaseTestDbContextActivator.CreateDbContext{TDbContext}(Action{DbContextOptionsBuilder{TDbContext}}?, Action{TDbContextOptionsBuilder}?)"/>
    public static TDbContext Create<TDbContext>(
        Action<DbContextOptionsBuilder<TDbContext>>? configureDbContextOptions,
        Action<SqliteDbContextOptionsBuilder>? configureProviderOptions) where TDbContext : DbContext =>
        instance.CreateDbContext(configureDbContextOptions, configureProviderOptions);

    override protected string GetDatabaseName(Type dbContextType)
    {
        Assert.That(dbContextType != null);

        return $"{dbContextType!.Name}_{Guid.NewGuid()}.sqlite";
    }

    override protected DbConnection CreateConnection(string databasePath)
    {
        Assert.That(databasePath != null);

        var connectionString = $"Data Source={databasePath};";
        return new SqliteConnection(connectionString);
    }

    override protected DbContextOptions<TDbContext> ConfigureDbContextOptions<TDbContext>(
        DbConnection dbConnection,
        Action<DbContextOptionsBuilder<TDbContext>>? configureContextOptions,
        Action<SqliteDbContextOptionsBuilder>? configureOptions)
    {
        Assert.That(dbConnection != null);

        var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();
        optionsBuilder.UseSqlite(dbConnection, x => configureOptions?.Invoke(x));
        configureContextOptions?.Invoke(optionsBuilder);
        return optionsBuilder.Options;
    }

    override protected void CreateDatabase(string databasePath)
    {
        Assert.That(databasePath != null);

        File.WriteAllBytes(databasePath!, Array.Empty<byte>());
    }
}
