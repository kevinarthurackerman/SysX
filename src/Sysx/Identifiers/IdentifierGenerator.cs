namespace Sysx.Identifiers;

internal static class IdentifierGenerator
{
#if NET48
    internal static Guid Next(IdentifierType identifierType, IdentifierOptions? options = null)
    {
        options ??= new();

        var randomBytes = new byte[10];
        options.Value.SetRandomBytes(randomBytes);

        var timestamp = options.Value.GetNow().Ticks / 10000L;
        var timestampBytes = BitConverter.GetBytes(timestamp);

        if (BitConverter.IsLittleEndian) Array.Reverse(timestampBytes);

        var guidBytes = new byte[16];

        switch (identifierType)
        {
            case IdentifierType.String:
            case IdentifierType.Binary:
                Buffer.BlockCopy(timestampBytes, 2, guidBytes, 0, 6);
                Buffer.BlockCopy(randomBytes, 0, guidBytes, 6, 10);

                // If formatting as a string, we have to reverse the order
                // of the Data1 and Data2 blocks on little-endian systems.
                if (identifierType == IdentifierType.String && BitConverter.IsLittleEndian)
                {
                    Array.Reverse(guidBytes, 0, 4);
                    Array.Reverse(guidBytes, 4, 2);
                }
                break;

            case IdentifierType.SqlServer:
                Buffer.BlockCopy(randomBytes, 0, guidBytes, 0, 10);
                Buffer.BlockCopy(timestampBytes, 2, guidBytes, 10, 6);
                break;
        }

        return new Guid(guidBytes);
    }
#endif

#if NET6_0 || NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
    internal static Guid Next(IdentifierType identifierType, IdentifierOptions? options = null)
    {
        options ??= new();

        Span<byte> randomBytes = stackalloc byte[10];
        options.Value.SetRandomBytes(randomBytes, null);

        var timestamp = options.Value.GetNow().Ticks / 10000L;

        Span<byte> timestampSpan = stackalloc byte[8];
        MemoryMarshal.Write(timestampSpan, ref timestamp);

        if (BitConverter.IsLittleEndian) timestampSpan.Reverse();

        Span<byte> guidBytes = stackalloc byte[16];

        switch (identifierType)
        {
            case IdentifierType.String:
            case IdentifierType.Binary:
                timestampSpan.Slice(2, 6).CopyTo(guidBytes.Slice(0, 6));
                randomBytes.Slice(0, 10).CopyTo(guidBytes.Slice(6, 10));

                // If formatting as a string, we have to reverse the order
                // of the Data1 and Data2 blocks on little-endian systems.
                if (identifierType == IdentifierType.String && BitConverter.IsLittleEndian)
                {
                    guidBytes.Slice(0, 4).Reverse();
                    guidBytes.Slice(4, 2).Reverse();
                }
                break;

            case IdentifierType.SqlServer:
                randomBytes.Slice(0, 10).CopyTo(guidBytes.Slice(0, 10));
                timestampSpan.Slice(2, 6).CopyTo(guidBytes.Slice(10, 6));
                break;
        }

        return new Guid(guidBytes);
    }
#endif
}