namespace Sysx.EntityFramework.Plugins;

public abstract class BaseRelationalMappingForType : RelationalTypeMapping
{
    public abstract Type ForType { get; }

    protected BaseRelationalMappingForType(RelationalTypeMappingParameters parameters) : base(parameters) { }

    protected BaseRelationalMappingForType(
        string storeType,
        Type clrType,
        DbType? dbType = null,
        bool unicode = false,
        int? size = null,
        bool fixedLength = false,
        int? precision = null,
        int? scale = null)
        : base(storeType, clrType, dbType, unicode, size, fixedLength, precision, scale) { }
}
