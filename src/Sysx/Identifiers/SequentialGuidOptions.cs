namespace Sysx.Identifiers;

public struct SequentialGuidOptions
{
    private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

    private static readonly Func<DateTime> defaultGetNow = () => DateTime.UtcNow;

#if NET48
    private static readonly Action<byte[]> setRandomBytes = bytes => _rng.GetBytes(bytes);
#elif NET6_0 || NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
    private static readonly SpanAction<byte, object?> setRandomBytes = (bytes, state) => _rng.GetBytes(bytes);
#endif

    public Func<DateTime> GetNow { get; set; } = defaultGetNow;

#if NET48
    public Action<byte[]> SetRandomBytes { get; set; } = setRandomBytes;
#elif NET6_0 || NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
    public SpanAction<byte, object?> SetRandomBytes { get; set; } = setRandomBytes;
#endif
}