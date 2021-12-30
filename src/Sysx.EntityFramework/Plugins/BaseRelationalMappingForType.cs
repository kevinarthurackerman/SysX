namespace Sysx.EntityFramework.Plugins;

/// <summary>
/// Base class for implementing container registered RelationalTypeMapping.
/// </summary>
public abstract class BaseRelationalMappingForType : RelationalTypeMapping
{
    /// <summary>
    /// The Type that this RelationalTypeMapping will be applied to.
    /// </summary>
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
