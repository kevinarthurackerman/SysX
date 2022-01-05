namespace Sysx.EntityFramework;

public static class IDatabaseProviderExtensions
{
    private const string Microsoft_EntityFrameworkCore_InMemory = "Microsoft.EntityFrameworkCore.InMemory";
    private const string Microsoft_EntityFrameworkCore_Sqlite = "Microsoft.EntityFrameworkCore.Sqlite";
    private const string Microsoft_EntityFrameworkCore_SqlServer = "Microsoft.EntityFrameworkCore.SqlServer";

    /// <summary>
    /// Returns true if the database provider is known to be an InMemory database.
    /// </summary>
    public static bool IsInMemory(this IDatabaseProvider databaseProvider)
    {
        EnsureArg.IsNotNull(databaseProvider, nameof(databaseProvider));

        return databaseProvider.Name == Microsoft_EntityFrameworkCore_InMemory;
    }

    /// <summary>
    /// Returns true if the database provider is known to be a Sqlite database.
    /// </summary>
    public static bool IsSqlite(this IDatabaseProvider databaseProvider)
    {
        EnsureArg.IsNotNull(databaseProvider, nameof(databaseProvider));

        return databaseProvider.Name == Microsoft_EntityFrameworkCore_Sqlite;
    }


    /// <summary>
    /// Returns true if the database provider is known to be a SqlServer database.
    /// </summary>
    public static bool IsSqlServer(this IDatabaseProvider databaseProvider)
    {
        EnsureArg.IsNotNull(databaseProvider, nameof(databaseProvider));
        
        return databaseProvider.Name == Microsoft_EntityFrameworkCore_SqlServer;
    }
}