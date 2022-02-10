namespace SysX;

/// <summary>
/// Static class used to assert that certain conditions should always be met, but to only check for them during debugging.
/// </summary>
public static class Assert
{
    /// <summary>
    /// The compilation symbol to declare in order to include <see cref="Assert"/> checks in the compiled code.
    /// </summary>
    public const string CompilationSymbol = "ASSERTIONS";

    /// <summary>
    /// The context in which <see cref="Assert.That(bool, Func{Context, string}?, string?, int?, string?, string?)"/> is being called.
    /// </summary>
    public readonly record struct Context(string? FilePath, int? LineNumber, string? MemberName, string? Expression);

#if NET6_0 || NET5_0 || NETCOREAPP3_1
    /// <summary>
    ///  Specifies a contract that must be met and throws a <see cref="ContractException"/> if violated.
    ///  Check is only included in the source when the ASSERTIONS compilation symbol is included.
    /// </summary>
    [Conditional(CompilationSymbol)]
    public static void That(
        bool condition,
        Func<Context, string>? message = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int? callerLineNumber = null,
        [CallerMemberName] string? callerMemberName = null,
        [CallerArgumentExpression("condition")] string? conditionArgumentExpression = null)
    {
#if ASSERTIONS
        if (!condition)
        {
            var options = new Context(callerFilePath, callerLineNumber, callerMemberName, conditionArgumentExpression);
            message ??= context => $"Condition '{context.Expression}' failed at {context.FilePath} line {context.LineNumber}: {context.MemberName}";
            throw new ContractException(message(options));
        }
#endif
    }
#endif

#if NETSTANDARD2_1 || NET48
    /// <summary>
    ///  Specifies a contract that must be met and throws a <see cref="ContractException"/> if violated.
    ///  Check is only included in the source when the ASSERTIONS compilation symbol is included.
    /// </summary>
    [Conditional(CompilationSymbol)]
    public static void That(
        bool condition,
        Func<Context, string>? message = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int? callerLineNumber = null,
        [CallerMemberName] string? callerMemberName = null,
        string? conditionArgumentExpression = null)
    {
#if ASSERTIONS
        if (!condition)
        {
            var options = new Context(callerFilePath, callerLineNumber, callerMemberName, null);
            message ??= context => $"Condition failed at {context.FilePath} line {context.LineNumber}: {context.MemberName}";
            throw new ContractException(message(options));
        }
#endif
    }
#endif
}
