namespace Sysx.EntityFramework.Testing;

public static class SqlServerTestDbContextActivator
{
    private static readonly string workingDirectory;

    private static readonly ConcurrentDictionary<Type, string> baseDatabasePathLocator = new();

    static SqlServerTestDbContextActivator()
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

            return Path.Combine(new string[] { Environment.CurrentDirectory, "Temp", tempFilesRoot, databasesDirectory });
        }
    }

    public static TDbContext Create<TDbContext>()
         where TDbContext : DbContext
    {
        var sourceDatabasePath = baseDatabasePathLocator.GetOrAdd(typeof(TDbContext), dbContextType =>
        {
            var databasePath = Path.GetFullPath($"{workingDirectory}/{dbContextType.Name}_{Guid.NewGuid()}.mdf");

            CreateDatabase(databasePath);

            using var dbContext = CreateDbContext(databasePath);

            dbContext!.Database.EnsureCreated();

            return databasePath;
        });

        var destinationDatabasePath = Path.GetFullPath($"{workingDirectory}/{typeof(TDbContext).Name}_{Guid.NewGuid()}.mdf");

        CopyDatabase(sourceDatabasePath, destinationDatabasePath);

        return CreateDbContext(destinationDatabasePath);

        static void CopyDatabase(string sourceDatabasePath, string destinationDatabasePath)
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

        static TDbContext CreateDbContext(string databasePath)
        {
            var connectionString = $"Data Source=(LocalDB)\\MSSQLLocalDB; AttachDbFilename={databasePath}; Integrated Security=True; Connect Timeout=30; Pooling=false;";
            var connection = new SqlConnection(connectionString);

            var optionsCtor = typeof(TDbContext).GetConstructor(new[] { typeof(DbContextOptions<TDbContext>) })
                ?? typeof(TDbContext).GetConstructor(new[] { typeof(DbContextOptions) });

            if (optionsCtor != null)
            {
                var options = new DbContextOptionsBuilder<TDbContext>()
                    .UseSqlServer(connection)
                    .Options;

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

        static void CreateDatabase(string databasePath)
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
}
