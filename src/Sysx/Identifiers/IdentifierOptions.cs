namespace Sysx.Identifiers;

/// <summary>
/// Options for generating a semi-sequential <see cref="Guid"/>.
/// </summary>
public readonly record struct IdentifierOptions
{
    private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

    private static readonly Func<DateTime> defaultGetNow = () => DateTime.UtcNow;

#if NET48
    private static readonly Action<byte[]> setRandomBytes = bytes => _rng.GetBytes(bytes);

    public static IdentifierOptions Default => new(defaultGetNow, setRandomBytes);

    /// <summary>
    /// Initializes instance of <see cref="IdentifierOptions"/>.
    /// </summary>
    public IdentifierOptions(Func<DateTime> getNow, Action<byte[]> setRandomBytes)
    {
        GetNow = getNow;
        SetRandomBytes = setRandomBytes;
    }
#elif NET6_0 || NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
    private static readonly SpanAction<byte, object?> setRandomBytes = (bytes, state) => _rng.GetBytes(bytes);

    public static IdentifierOptions Default => new(defaultGetNow, setRandomBytes);

    /// <summary>
    /// Initializes instance of <see cref="IdentifierOptions"/>.
    /// </summary>
    public IdentifierOptions(Func<DateTime> getNow, SpanAction<byte, object?> setRandomBytes)
    {
        GetNow = getNow;
        SetRandomBytes = setRandomBytes;
    }
#endif

    /// <summary>
    /// <see cref="Func{TResult}"/> the gets the current "now" value.
    /// </summary>
    public Func<DateTime> GetNow { get; init; }

#if NET48
    /// <summary>
    /// <see cref="Action{T}"/> that sets random <see langword="byte[]"/> values.
    /// </summary>
    public Action<byte[]> SetRandomBytes { get; init; }
#elif NET6_0 || NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
    /// <summary>
    /// <see cref="SpanAction{T, TArg}"/> that sets random <see langword="byte[]"/> values.
    /// </summary>
    public SpanAction<byte, object?> SetRandomBytes { get; init; }
#endif
}