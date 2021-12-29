namespace Sysx.EntityFramework.SqlServer.Testing;

public sealed class SqlServerTestDbContextActivator : BaseTestDbContextActivator<SqlServerDbContextOptionsBuilder>
{
    private static readonly SqlServerTestDbContextActivator instance = new();

    private SqlServerTestDbContextActivator() { }

    /// <inheritdoc cref="BaseTestDbContextActivator.CreateDbContext{TDbContext}(Action{DbContextOptionsBuilder{TDbContext}}?)"/>
    public static TDbContext Create<TDbContext>() where TDbContext : DbContext =>
        instance.CreateDbContext<TDbContext>();

    /// <inheritdoc cref="BaseTestDbContextActivator.CreateDbContext{TDbContext}(Action{DbContextOptionsBuilder{TDbContext}}?)"/>
    public static TDbContext Create<TDbContext>(
        Action<DbContextOptionsBuilder<TDbContext>>? configureContextOptions,
        Action<SqlServerDbContextOptionsBuilder>? configureProviderOptions) where TDbContext : DbContext =>
        instance.CreateDbContext(configureContextOptions, configureProviderOptions);

    override protected string GetDatabaseName(Type dbContextType)
    {
        Assert.That(dbContextType != null);

        return $"{dbContextType!.Name}_{Guid.NewGuid()}.mdf";
    }

    override protected DbConnection CreateConnection(string databasePath)
    {
        Assert.That(databasePath != null);

        var connectionString = $"Data Source=(LocalDB)\\MSSQLLocalDB; AttachDbFilename={databasePath}; Integrated Security=True; Connect Timeout=30; Pooling=false;";
        return new SqlConnection(connectionString);
    }

    override protected DbContextOptions<TDbContext> ConfigureDbContextOptions<TDbContext>(
        DbConnection dbConnection,
        Action<DbContextOptionsBuilder<TDbContext>>? configureContextOptions,
        Action<SqlServerDbContextOptionsBuilder>? configureOptions)
    {
        Assert.That(dbConnection != null);
        Assert.That(configureContextOptions != null);
        Assert.That(configureOptions != null);

        var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();
        optionsBuilder.UseSqlServer(dbConnection, x => configureOptions?.Invoke(x));
        configureContextOptions?.Invoke(optionsBuilder);
        return optionsBuilder.Options;
    }

    override protected void CreateDatabase(string databasePath)
    {
        Assert.That(databasePath != null);

        var databaseName = Path.GetFileNameWithoutExtension(databasePath);
        using var connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB; Initial Catalog=master; Integrated Security=true;");

        connection.Open();

        using var command = connection.CreateCommand();

        command.CommandText = $"CREATE DATABASE [{databaseName}] ON PRIMARY (NAME='{databaseName}', FILENAME='{databasePath}')";
        command.ExecuteNonQuery();

        command.CommandText = $"EXEC sp_detach_db '{databaseName}', 'true'";
        command.ExecuteNonQuery();
    }
}
