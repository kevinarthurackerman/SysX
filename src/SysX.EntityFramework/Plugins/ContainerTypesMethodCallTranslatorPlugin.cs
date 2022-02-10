namespace SysX.EntityFramework.Plugins;

/// <summary>
/// The IMethodCallTranslatorPlugin used by ContainerTypesDbContextOptionsExtensions to handle IMethodCallTranslators
/// </summary>
public class ContainerTypesMethodCallTranslatorPlugin : IMethodCallTranslatorPlugin
{
    public ContainerTypesMethodCallTranslatorPlugin(IEnumerable<IMethodCallTranslator> methodCallTranslators)
    {
        EnsureArg.IsNotNull(methodCallTranslators, nameof(methodCallTranslators));

        var baseCallTranslatorForMethods = methodCallTranslators
            .OfType<BaseCallTranslatorForMethod>()
            .ToArray();

        var callTranslatorForMethods = new CallTranslatorForMethods(baseCallTranslatorForMethods);

        var otherMethodCallTranslators = methodCallTranslators
            .Where(x => x is not BaseCallTranslatorForMethod)
            .ToArray();

        Translators = otherMethodCallTranslators.Prepend(callTranslatorForMethods).ToArray();
    }

    public IEnumerable<IMethodCallTranslator> Translators { get; }

    private class CallTranslatorForMethods : IMethodCallTranslator
    {
        private readonly ImmutableDictionary<MethodInfo, BaseCallTranslatorForMethod> translatorLookupByMember;

        internal CallTranslatorForMethods(IEnumerable<BaseCallTranslatorForMethod> baseCallTranslatorForMethods)
        {
            Debug.Assert(baseCallTranslatorForMethods != null);

            translatorLookupByMember = baseCallTranslatorForMethods!.ToImmutableDictionary(x => x.ForMethod);
        }

        public SqlExpression? Translate(
            SqlExpression instance,
            MethodInfo method,
            IReadOnlyList<SqlExpression> arguments,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            translatorLookupByMember.TryGetValue(method, out var translator);

            return translator?.Translate(instance, method, arguments, logger);
        }
    }
}