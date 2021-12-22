namespace Sysx;

public static class Assert
{
    public const string CompilationSymbol = "ASSERTIONS";

#if NET5_0 || NETCOREAPP3_1
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
    public static void That(bool condition,
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
            message ??= x => $"Condition '{conditionArgumentExpression}' failed at {callerFilePath} line {callerLineNumber}: {callerMemberName}";
            throw new ContractException(message(options));
        }
#endif
    }

    public readonly record struct Context
    {
        public string? FilePath { get; }
        public int? LineNumber { get; }
        public string? MemberName { get; }
        public string? ConditionArgumentExpression { get; }

        internal Context(string? filePath, int? lineNumber, string? memberName, string? conditionArgumentExpression)
        {
            FilePath = filePath;
            LineNumber = lineNumber;
            MemberName = memberName;
            ConditionArgumentExpression = conditionArgumentExpression;
        }
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
    public static void That(bool condition,
        Func<Context, string>? message = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int? callerLineNumber = null,
        [CallerMemberName] string? callerMemberName = null,
        string? conditionArgumentExpression = null)
    {
#if ASSERTIONS
        if (!condition)
        {
            var options = new Context(callerFilePath, callerLineNumber, callerMemberName);
            message ??= x => $"Condition failed at {callerFilePath} line {callerLineNumber}: {callerMemberName}";
            throw new ContractException(message(options));
        }
#endif
    }

    public class Context
    {
        public string? FilePath { get; }
        public int? LineNumber { get; }
        public string? MemberName { get; }

        internal Context(string? filePath, int? lineNumber, string? memberName)
        {
            FilePath = filePath;
            LineNumber = lineNumber;
            MemberName = memberName;
        }
    }
#endif
}