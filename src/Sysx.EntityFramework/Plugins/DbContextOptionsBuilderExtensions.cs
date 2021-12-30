namespace Sysx.EntityFramework.Plugins;

public static class DbContextOptionsBuilderExtensions
{
    /// <summary>
    /// Checks if the extension already exists and adds it if it does not.
    /// </summary>
    public static bool TryAddExtension<TExtension>(this IRelationalDbContextOptionsBuilderInfrastructure dbContextOptionsBuilder, TExtension extension)
        where TExtension : class, IDbContextOptionsExtension
    {
        EnsureArg.IsNotNull(dbContextOptionsBuilder, nameof(dbContextOptionsBuilder));
        EnsureArg.IsNotNull(extension, nameof(extension));

        var coreOptionsBuilder = dbContextOptionsBuilder.OptionsBuilder;

        if (coreOptionsBuilder.Options.FindExtension<TExtension>() == null)
        {
            ((IDbContextOptionsBuilderInfrastructure)coreOptionsBuilder).AddOrUpdateExtension(extension);
            return true;
        }

        return false;
    }
}