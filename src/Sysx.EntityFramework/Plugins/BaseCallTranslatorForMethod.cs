namespace Sysx.EntityFramework.Plugins;

public abstract class BaseCallTranslatorForMethod : IMethodCallTranslator
{
    public abstract MethodInfo ForMethod { get; }

    public abstract SqlExpression Translate(
        SqlExpression instance,
        MethodInfo method,
        IReadOnlyList<SqlExpression> arguments,
        IDiagnosticsLogger<DbLoggerCategory.Query> logger);
}