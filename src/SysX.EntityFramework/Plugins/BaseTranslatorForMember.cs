namespace SysX.EntityFramework.Plugins;

/// <summary>
/// Base class for implementing container registered IMemberTranslator.
/// </summary>
public abstract class BaseTranslatorForMember : IMemberTranslator
{
	/// <summary>
	/// The MemberInfo that this IMemberTranslator will be applied to.
	/// </summary>
	public abstract MemberInfo ForMember { get; }

	public abstract SqlExpression Translate(
		SqlExpression instance,
		MemberInfo member,
		Type returnType,
		IDiagnosticsLogger<DbLoggerCategory.Query> logger);
}