namespace Sysx.EntityFramework.Testing;

public sealed class SqlServerTestDbContextActivator : BaseTestDbContextActivator
{
    private static readonly SqlServerTestDbContextActivator instance = new();

    private SqlServerTestDbContextActivator() { }

    /// <inheritdoc cref="BaseTestDbContextActivator.CreateDbContext{TDbContext}" />
    public static TDbContext Create<TDbContext>() where TDbContext : DbContext => instance.CreateDbContext<TDbContext>();

    override protected string GetDatabaseName(Type dbContextType)
    {
        return $"{dbContextType.Name}_{Guid.NewGuid()}.mdf";
    }

    override protected DbConnection CreateConnection(string databasePath)
    {
        var connectionString = $"Data Source=(LocalDB)\\MSSQLLocalDB; AttachDbFilename={databasePath}; Integrated Security=True; Connect Timeout=30; Pooling=false;";
        return new SqlConnection(connectionString);
    }

    override protected DbContextOptions<TDbContext> CreateDbContextOptions<TDbContext>(DbConnection dbConnection)
    {
        return new DbContextOptionsBuilder<TDbContext>()
                .UseSqlServer(dbConnection)
                .Options;
    }

    override protected void CreateDatabase(string databasePath)
    {
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
