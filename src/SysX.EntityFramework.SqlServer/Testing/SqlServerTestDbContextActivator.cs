namespace SysX.EntityFramework.SqlServer.Testing;

public sealed class SqlServerTestDbContextActivator
    : BaseTestDbContextActivator<SqlServerDbContextOptionsBuilder, SqlServerDbContextOptionsBuilder, SqlServerOptionsExtension>
{
    private static readonly SqlServerTestDbContextActivator instance = new();

    private SqlServerTestDbContextActivator() { }

    /// <inheritdoc cref="BaseTestDbContextActivator.CreateDbContext{TDbContext}(Action{DbContextOptionsBuilder{TDbContext}}?, Action{TDbContextOptionsBuilder}?)"/>
    public static TDbContext Create<TDbContext>() where TDbContext : DbContext =>
        instance.CreateDbContext<TDbContext>();

    /// <inheritdoc cref="BaseTestDbContextActivator.CreateDbContext{TDbContext}(Action{DbContextOptionsBuilder{TDbContext}}?, Action{TDbContextOptionsBuilder}?)"/>
    public static TDbContext Create<TDbContext>(
        Action<CreateDbContextOptions<TDbContext>>? configure) where TDbContext : DbContext =>
        instance.CreateDbContext(configure);

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
        Action<CreateDbContextOptions<TDbContext>>? configure)
    {
        Assert.That(dbConnection != null);

        var dbContextOptions = new DbContextOptionsBuilder<TDbContext>();
        dbContextOptions.UseSqlServer(dbConnection, providerOptions =>
            configure?.Invoke(new CreateDbContextOptions<TDbContext>(dbContextOptions, providerOptions)));
        return dbContextOptions.Options;
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
