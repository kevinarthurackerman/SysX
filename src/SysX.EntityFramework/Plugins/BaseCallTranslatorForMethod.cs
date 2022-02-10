namespace SysX.EntityFramework.Plugins;

/// <summary>
/// Base class for implementing container registered IMethodCallTranslators.
/// </summary>
public abstract class BaseCallTranslatorForMethod : IMethodCallTranslator
{
    /// <summary>
    /// The MethodInfo that this IMethodCallTranslator will be applied to.
    /// </summary>
    public abstract MethodInfo ForMethod { get; }

    public abstract SqlExpression Translate(
        SqlExpression instance,
        MethodInfo method,
        IReadOnlyList<SqlExpression> arguments,
        IDiagnosticsLogger<DbLoggerCategory.Query> logger);
}