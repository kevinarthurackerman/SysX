namespace SysX.EntityFramework.Sqlite.Testing;

public sealed class SqliteTestDbContextActivator
	: BaseTestDbContextActivator<SqliteDbContextOptionsBuilder, SqliteDbContextOptionsBuilder, SqliteOptionsExtension>
{
	private static readonly SqliteTestDbContextActivator instance = new();

	private SqliteTestDbContextActivator() { }

	/// <inheritdoc cref="BaseTestDbContextActivator.CreateDbContext{TDbContext}(Action{DbContextOptionsBuilder{TDbContext}}?, Action{TDbContextOptionsBuilder}?)"/>
	public static TDbContext Create<TDbContext>() where TDbContext : DbContext =>
		instance.CreateDbContext<TDbContext>();

	/// <inheritdoc cref="BaseTestDbContextActivator.CreateDbContext{TDbContext}(Action{DbContextOptionsBuilder{TDbContext}}?, Action{TDbContextOptionsBuilder}?)"/>
	public static TDbContext Create<TDbContext>(Action<CreateDbContextOptions<TDbContext>>? configure) where TDbContext : DbContext =>
		instance.CreateDbContext(configure);

	override protected string GetDatabaseName(Type dbContextType)
	{
		Debug.Assert(dbContextType != null);

		return $"{dbContextType!.Name}_{Guid.NewGuid()}.sqlite";
	}

	override protected DbConnection CreateConnection(string databasePath)
	{
		Debug.Assert(databasePath != null);

		var connectionString = $"Data Source={databasePath};";
		return new SqliteConnection(connectionString);
	}

	override protected DbContextOptions<TDbContext> ConfigureDbContextOptions<TDbContext>(
		DbConnection dbConnection,
		Action<CreateDbContextOptions<TDbContext>>? configure)
	{
		Debug.Assert(dbConnection != null);

		var dbContextOptions = new DbContextOptionsBuilder<TDbContext>();
		dbContextOptions.UseSqlite(dbConnection, providerOptions =>
			configure?.Invoke(new CreateDbContextOptions<TDbContext>(dbContextOptions, providerOptions)));
		return dbContextOptions.Options;
	}

	override protected void CreateDatabase(string databasePath)
	{
		Debug.Assert(databasePath != null);

		File.WriteAllBytes(databasePath!, Array.Empty<byte>());
	}
}
