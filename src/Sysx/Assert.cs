namespace Sysx;

public static class Assert
{
    public const string CompilationSymbol = "ASSERTIONS";

    public readonly record struct Context(string? FilePath, int? LineNumber, string? MemberName, string? Expression);

#if NET6_0 || NET5_0 || NETCOREAPP3_1
    /// <summary>
    ///  Specifies a contract that must be met and throws an exception if violated
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="message"></param>
    /// <param name="callerFilePath">Automatically provided</param>
    /// <param name="callerLineNumber">Automatically provided</param>
    /// <param name="callerMemberName">Automatically provided</param>
    /// <param name="conditionArgumentExpression">Automatically provided</param>
    /// <exception cref="ContractException"></exception>

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
    ///  Specifies a contract that must be met and throws an exception if violated
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="message"></param>
    /// <param name="callerFilePath">Automatically provided</param>
    /// <param name="callerLineNumber">Automatically provided</param>
    /// <param name="callerMemberName">Automatically provided</param>
    /// <param name="conditionArgumentExpression">This argument is not supported in net4.8 or netstandard2.1</param>
    /// <exception cref="ContractException"></exception>
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
