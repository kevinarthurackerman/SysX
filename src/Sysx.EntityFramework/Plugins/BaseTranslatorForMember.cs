namespace Sysx.EntityFramework.Plugins;

public abstract class BaseTranslatorForMember : IMemberTranslator
{
    public abstract MemberInfo ForMember { get; }

    public abstract SqlExpression Translate(
        SqlExpression instance,
        MemberInfo member,
        Type returnType,
        IDiagnosticsLogger<DbLoggerCategory.Query> logger);
}