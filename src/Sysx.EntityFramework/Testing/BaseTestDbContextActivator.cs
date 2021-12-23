namespace Sysx.EntityFramework.Testing;

public abstract class BaseTestDbContextActivator
{
    private static readonly string workingDirectory;

    private static readonly ConcurrentDictionary<DatabasePathLocator, string> baseDatabasePathLocator = new();

    static BaseTestDbContextActivator()
    {
        workingDirectory = GetWorkingDirectory();

        Directory.CreateDirectory(workingDirectory);

        CleanWorkingDirectory();
        
        AppDomain.CurrentDomain.ProcessExit += (s, e) => CleanWorkingDirectory();

        static void CleanWorkingDirectory()
        {
            foreach (var file in Directory.GetFiles(workingDirectory))
                try { File.Delete(file); } catch (Exception) { }
        }

        static string GetWorkingDirectory()
        {
            var tempFilesRoot = "DbContextActivator";
            var databasesDirectory = "TestDatabases";
            
            try
            {
                var basePath = Path.GetTempPath();

                if (!string.IsNullOrEmpty(basePath))
                {
                    return Path.Combine(basePath, tempFilesRoot, databasesDirectory);
                }
            }
            catch (Exception) { }

            return Path.Combine(Environment.CurrentDirectory, "Temp", tempFilesRoot, databasesDirectory);
        }
    }

    /// <summary>
    /// Creates a test database using the default EntityFramework migrations and returns a DbContext attached to that database.
    /// A new test database is created each time this is called and is torn down at the end of the program execution.
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    protected TDbContext CreateDbContext<TDbContext>() where TDbContext : DbContext
    {
        var baseDatabasePathLocatorKey = new DatabasePathLocator(GetType(), typeof(TDbContext));
        var sourceDatabasePath = baseDatabasePathLocator.GetOrAdd(baseDatabasePathLocatorKey, key =>
        {
            var databasePath = Path.Combine(workingDirectory, GetDatabaseName(key.DbContextType));

            CreateDatabase(databasePath);

            using var dbContext = CreateDbContext(databasePath);

            dbContext!.Database.EnsureCreated();

            return databasePath;
        });

        var destinationDatabasePath = Path.Combine(workingDirectory, GetDatabaseName(typeof(TDbContext)));

        CopyDatabase(sourceDatabasePath, destinationDatabasePath);

        return CreateDbContext(destinationDatabasePath);

        void CopyDatabase(string sourceDatabasePath, string destinationDatabasePath)
        {
            var attempts = 0;
            while (attempts < 30)
            {
                try
                {
                    File.Copy(sourceDatabasePath, destinationDatabasePath, true);
                    return;
                }
                catch (Exception)
                {
                    attempts++;
                    Thread.Sleep(100);
                }
            }

            throw new TimeoutException($"Failed to copy database from {sourceDatabasePath} to {destinationDatabasePath}");
        }

        TDbContext CreateDbContext(string databasePath)
        {
            var connection = CreateConnection(databasePath);

            var optionsCtor = typeof(TDbContext).GetConstructor(new[] { typeof(DbContextOptions<TDbContext>) })
                ?? typeof(TDbContext).GetConstructor(new[] { typeof(DbContextOptions) });

            if (optionsCtor != null)
            {
                var options = CreateDbContextOptions<TDbContext>(connection);

                return (TDbContext)optionsCtor.Invoke(new[] { options });
            }

            var optionlessCtor = typeof(TDbContext).GetConstructor(Array.Empty<Type>());

            if (optionlessCtor != null)
            {
                var dbContext = (TDbContext)optionlessCtor.Invoke(Array.Empty<object>());

                dbContext.Database.SetDbConnection(connection);

                return dbContext;
            }

            throw new InvalidOperationException($"{nameof(DbContext)} {typeof(TDbContext).Name} does not have a parameterless constructor or a constructor that takes a single parameter of type {typeof(DbContextOptions).Name} or {typeof(DbContextOptions<TDbContext>).Name}");
        }
    }

    protected abstract string GetDatabaseName(Type dbContextType);

    protected abstract DbConnection CreateConnection(string databasePath);

    protected abstract DbContextOptions<TDbContext> CreateDbContextOptions<TDbContext>(DbConnection dbConnection)
        where TDbContext : DbContext;

    protected abstract void CreateDatabase(string databasePath);

    private record struct DatabasePathLocator(Type ActivatorType, Type DbContextType);
}
