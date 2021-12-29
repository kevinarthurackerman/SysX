﻿namespace Sysx.EntityFramework.Plugins;

public class ContainerTypesMemberTranslatorPlugin : IMemberTranslatorPlugin
{
    public ContainerTypesMemberTranslatorPlugin(IEnumerable<IMemberTranslator> memberTranslators)
    {
        EnsureArg.IsNotNull(memberTranslators, nameof(memberTranslators));

        var baseTranslatorForMembers = memberTranslators
            .Where(x => x is BaseTranslatorForMember)
            .Cast<BaseTranslatorForMember>()
            .ToArray();

        var translatorForMembers = new TranslatorForMembers(baseTranslatorForMembers);

        var otherMemberTranslators = memberTranslators
            .Where(x => x is not BaseTranslatorForMember)
            .ToArray();

        Translators = otherMemberTranslators.Prepend(translatorForMembers).ToArray();
    }

    public IEnumerable<IMemberTranslator> Translators { get; }

    private class TranslatorForMembers : IMemberTranslator
    {
        private readonly ImmutableDictionary<MemberInfo, BaseTranslatorForMember> translatorLookupByMember;

        internal TranslatorForMembers(IEnumerable<BaseTranslatorForMember> baseTranslatorForMembers)
        {
            Assert.That(baseTranslatorForMembers != null);

            translatorLookupByMember = baseTranslatorForMembers!.ToImmutableDictionary(x => x.ForMember);
        }

        public SqlExpression? Translate(
            SqlExpression instance,
            MemberInfo member,
            Type returnType,
            IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            translatorLookupByMember.TryGetValue(member, out var translator);

            return translator?.Translate(instance, member, returnType, logger);
        }
    }
}